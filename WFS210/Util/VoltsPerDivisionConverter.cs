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

		public static double ToVolts(VoltsPerDivision voltsPerDivision, AttenuationFactor attenuationFactor)
		{
			var volts = voltsPerDivisionValues [(int)voltsPerDivision];

			if (attenuationFactor == AttenuationFactor.X10)
				volts *= 10;
			return volts ;
		}

		public static string ToString(VoltsPerDivision voltsPerDivision, AttenuationFactor attenuationFactor)
		{
			double volts = VoltsPerDivisionConverter.ToVolts (voltsPerDivision, attenuationFactor);

			string caption;

			if (volts.Equals(0)) {
				caption = "Off"; // TODO: localize
			} else {
				caption = String.Format ("{0}/div", VoltageConverter.ToString (volts, 0));
			}

			return caption;
		}
	}
}

