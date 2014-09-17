using System;

namespace WFS210
{
	public static class VoltsPerDivisionConverter
	{
		/// <summary>
		/// Volts per division lookup table. All values are in volts (V).
		/// </summary>
		private static double[] voltsPerDivisionValues = new double[] {
			0, 20, 10, 4, 2, 1, 0.5, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005 
		};

		public static double ToVolts(VoltsPerDivision voltsPerDivision)
		{
			return voltsPerDivisionValues [(int)voltsPerDivision];
		}

		public static string ToString(VoltsPerDivision voltsPerDivision)
		{
			double volts = VoltsPerDivisionConverter.ToVolts (voltsPerDivision);

			string caption;

			if (volts == 0) {
				caption = "None"; // TODO: localize
			} else {
				caption = String.Format ("{0}/div", VoltageConverter.ToString (volts));
			}

			return caption;
		}
	}
}

