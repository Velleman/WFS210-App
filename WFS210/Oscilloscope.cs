using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WFS210
{
	/// <summary>
	/// WFS210 Oscilloscope.
	/// </summary>
	public class Oscilloscope
	{
		/// <summary>
		/// The oscilloscope's channels. This list should only be modified internally.
		/// </summary>
		private List<Channel> channels = new List<Channel>();

		/// <summary>
		/// Gets the device context containing device-specific information.
		/// </summary>
		/// <value>The context.</value>
		public DeviceContext DeviceContext { get; private set; }

		/// <summary>
		/// Gets the channels.
		/// </summary>
		/// <value>The channels.</value>
		public ReadOnlyCollection<Channel> Channels {
			get { return channels.AsReadOnly (); }
		}

		/// <summary>
		/// Gets or sets the time base.
		/// </summary>
		/// <value>The time base.</value>
		public TimeBase TimeBase { get; set; }

		/// <summary>
		/// Gets the trigger.
		/// </summary>
		/// <value>The trigger.</value>
		public Trigger Trigger { get; private set; }

		/// <summary>
		/// Enables or disables the auto-range.
		/// </summary>
		/// <value><c>true</c> if auto range is enabled; otherwise, <c>false</c>.</value>
		public bool AutoRange { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the oscilloscope <see cref="WFS210.Oscilloscope"/> is on hold.
		/// </summary>
		/// <value><c>true</c> if on hold; otherwise, <c>false</c>.</value>
		public bool Hold { get; set; }

		/// <summary>
		/// Gets or sets the battery status.
		/// </summary>
		/// <value>The battery status.</value>
		public BatteryStatus BatteryStatus { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the oscilloscope <see cref="WFS210.Oscilloscope"/> is calibrating.
		/// </summary>
		/// <value><c>true</c> if it is currently calibrating; otherwise, <c>false</c>.</value>
		public bool Calibrating { get; set; }

		/// <summary>
		/// Adds a new channel that is fully configured for this type of oscilloscope.
		/// </summary>
		protected void AddChannel() {

			Channel channel = new Channel (4096, DeviceContext);
			channels.Add (channel);
		}

		public Oscilloscope ()
		{
			DeviceContext = new DeviceContext (this);
			DeviceContext.Divisions = 10;
			DeviceContext.UnitsPerDivision = 25;
			DeviceContext.CenterPosition = 128;
			DeviceContext.SampleMin = 3;
			DeviceContext.SampleMax = 252;

			Trigger = new Trigger (DeviceContext.CenterPosition, TriggerSlope.Rising, TriggerMode.Run, 0);

			AddChannel ();
			AddChannel ();

			InitDefaults ();
		}

		public void InitDefaults()
		{
			AutoRange = false;
			Hold = false;
			TimeBase = TimeBase.Tdiv1ms;
			BatteryStatus = BatteryStatus.Charged;
			Calibrating = false;
		}
		/// <summary>
		/// Calculates the DBGain of the two signals
		/// </summary>
		/// <returns>The dBGain</returns>
		public double DBGain()
		{
			return Channels [1].DBm () - Channels [0].DBm ();
		}
	}
}

