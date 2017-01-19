using System;

namespace WFS210
{
	public class DeviceContext
	{
		private readonly Oscilloscope device;

		/// <summary>
		/// Gets the device for this context.
		/// </summary>
		/// <value>The device.</value>
		public Oscilloscope Device {
			get { return device; }
		}

		/// <summary>
		/// The number of divisions.
		/// </summary>
		public int Divisions { get; set; }

		/// <summary>
		/// The number of units per division.
		/// </summary>
		public int UnitsPerDivision { get; set; }

		/// <summary>
		/// Gets or sets the center position.
		/// </summary>
		/// <value>The center position.</value>
		public int CenterPosition { get; set; }

		/// <summary>
		/// Gets or sets the minimum sample value.
		/// </summary>
		/// <value>The minimum sample value.</value>
		public int SampleMin { get; set; }

		/// <summary>
		/// Gets or sets the maximum sample value.
		/// </summary>
		/// <value>The maximum sample value.</value>
		public int SampleMax { get; set; }

		/// <summary>
		/// Gets the number of samples for the current time base.
		/// </summary>
		/// <returns>The number of samples.</returns>
		public int SamplesPerTimeBase {

			get {
				switch (Device.TimeBase) {
				case TimeBase.Tdiv1us:
					return 10;
				case TimeBase.Tdiv2us:
					return 20;
				default:
					return 50;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.DeviceContext"/> class.
		/// </summary>
		/// <param name="device">Device.</param>
		public DeviceContext(Oscilloscope device)
		{
			this.device = device;
		}
	}
}

