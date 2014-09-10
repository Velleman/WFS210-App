using System;

namespace WFS210
{
	public class SignalGenerator
	{
		public SignalType Signal { get; set; }

		public float Frequency { get; set; }

		public float Amplitude { get; set; }

		public float Phase { get; set; }

		public float Offset { get; set; }

		public SignalGenerator ()
		{
			this.Signal = SignalType.Sine;
			this.Frequency = 100;
			this.Amplitude = 40;
			this.Phase = 0;
			this.Offset = 5;
		}

		public void Generate (Oscilloscope oscilloscope, int channelIndex)
		{
			Channel channel = oscilloscope.Channels[channelIndex];

			for (int i = 0; i < channel.Samples.Count; i++) {

				channel.Samples [i] = (byte)i;
			}
		}
	}
}

