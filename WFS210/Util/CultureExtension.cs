using System;

namespace WFS210.Util
{
	public static class CultureExtension
	{
		public static string FormatValue(double value, string[] units, double step)
		{
			int i = 0;

			while ((value < 0) && (i < units.Length)) {

				value *= step;
				i++;
			}

			return String.Format("{0}{1}", value, units[i]);
		}

		public static string Format(this VoltsPerDivision voltsPerDivision)
		{
			return CultureExtension.FormatValue (voltsPerDivision.GetValue (), new string[] { "V", "mV" }, 1000);
		}

		public static string Format(this TimeBase timeBase)
		{
			return CultureExtension.FormatValue (timeBase.GetValue (), new string[] { "s", "ms", "us" }, 1000);
		}
	}
}

