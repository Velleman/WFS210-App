using System;

namespace WFS210.Util
{
	/// <summary>
	/// Conversion extension. Supplies conversion tables and functions
	/// for enumerated oscilloscope settings.
	/// </summary>
	public static class ConversionExtension
	{
		/// <summary>
		/// Volts per division lookup table. All values are in volts (V).
		/// </summary>
		private static double[] voltsPerDivisionValues = new double[] {
			0, 20, 10, 4, 2, 1, 0.5, 0.2, 0.1, 0.05, 0.02, 0.01, 0.005 
		};

		/// <summary>
		/// Time base lookup table. All values are in seconds (s).
		/// </summary>
		private static double[] timeBaseValues = new double[] {
			0.000001, 0.000002, 0.000005,
			0.00001, 0.00002, 0.00005,
			0.0001, 0.0002, 0.0005,
			0.001, 0.002, 0.005,
			0.01, 0.02, 0.05,
			0.1, 0.2, 0.5,
			1
		};

		/// <summary>
		/// Converts a VoltsPerDivision enum to its real world value in volts (V).
		/// </summary>
		/// <returns>The enum's value in volts (V).</returns>
		/// <param name="voltsPerDivision">The volts per division that is to be converted to volts.</param>
		public static double GetValue(this VoltsPerDivision voltsPerDivision)
		{
			return voltsPerDivisionValues [(int)voltsPerDivision];
		}

		/// <summary>
		/// Converts a TimeBase enum to its real world value in seconds (s).
		/// </summary>
		/// <returns>The enum's value in seconds (s).</returns>
		/// <param name="timeBase">The time base that is to be converted to seconds.</param>
		public static double GetValue(this TimeBase timeBase)
		{
			return timeBaseValues [(int)timeBase];
		}
	}
}

