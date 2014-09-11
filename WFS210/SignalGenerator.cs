using System;

namespace WFS210
{
	public class SignalGenerator
	{
		public SignalType Signal { get; set; }

		public double Frequency { get; set; }

		public double Amplitude { get; set; }

		public double Phase { get; set; }

		public double Offset { get; set; }

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

			Random random = new Random ();
			double randomPhase = random.Next (0, 628) / 100;

			for (int i = 0; i < channel.Samples.Count; i++) {

				double gnd = 1, input = 0;

				double tl = oscilloscope.Trigger.Level;

				switch (channel.InputCoupling) {
				case InputCoupling.AC:
					gnd = 1;
					input = 0;
					break;
				case InputCoupling.DC:
					gnd = 1;
					input = 1;
					break;
				case InputCoupling.GND:
					gnd = 0;
					input = 0;
					break;
				}

				double samplesPerVolt = 25 / VoltsPerDivisionConverter.ToVolts (channel.VoltsPerDivision);

				double a = Amplitude * samplesPerVolt;
				double o = input * Offset * samplesPerVolt;

				double samplesPerDiv;

				switch (oscilloscope.TimeBase) {
				case TimeBase.Tdiv1us:
					samplesPerDiv = 10;
					break;
				case TimeBase.Tdiv2us:
					samplesPerDiv = 20;
					break;
				default:
					samplesPerDiv = 50;
					break;
				}

				double t = i * TimeBaseConverter.ToSeconds (oscilloscope.TimeBase) / samplesPerDiv;

				if ((Math.Floor(a) == 0) || (Math.Floor(gnd) == 0)) {
					Phase = 0;
				} else if ((tl < (channel.YPosition - (o + a))) || (tl > (channel.YPosition + (o + a)))) {
					Phase = randomPhase;
				} else if (oscilloscope.Trigger.Slope == TriggerSlope.Rising) {
					Phase = Math.Asin ((tl - (channel.YPosition - o)) / a);
				} else {
					Phase = Math.PI - Math.Asin ((tl - (channel.YPosition - o)) / a);
				}

				channel.Samples [i] = (byte)Math.Round(channel.YPosition - gnd * (o + a * Math.Sin(2 * Math.PI * Frequency * t + Phase)));
			}
		}
	}
}

