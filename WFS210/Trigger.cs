using System;

using WFS210.Util;

namespace WFS210
{
	/// <summary>
	/// Trigger.
	/// </summary>
	public class Trigger
	{
		private int level;

		/// <summary>
		/// Gets or sets the trigger slope.
		/// </summary>
		/// <value>The trigger slope.</value>
		public TriggerSlope Slope { get; set; }

		/// <summary>
		/// Gets or sets the triggering mode.
		/// </summary>
		/// <value>The triggering mode.</value>
		public TriggerMode Mode { get; set; }

		/// <summary>
		/// Gets or sets the trigger level (0..255).
		/// </summary>
		/// <value>The trigger level.</value>
		public int Level {

			get {
				return level;
			}
			set {
				level = value.LimitToRange (0, 255); // should use the device capabilities
			}
		}

		/// <summary>
		/// Gets or sets the channel that is used for triggering.
		/// </summary>
		/// <value>The trigger channel.</value>
		public int Channel { get; set; }//TODO: unclear what values can be given to channel 0 and 1 or 1 and 2

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.Trigger"/> class.
		/// </summary>
		/// <param name="level">Trigger level.</param>
		/// <param name="slope">Trigger slope.</param>
		/// <param name="mode">Trigger mode.</param>
		/// <param name="channel">Triggering channel.</param>
		public Trigger (int level, TriggerSlope slope, TriggerMode mode, int channel = 0)
		{
			Slope = slope;
			Level = level;
			Mode = mode;
			Channel = channel;
		}
	}
}

