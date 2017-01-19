using System;

namespace WFS210
{
	public static class FrequencyConverter
	{
		public static string ToString(double seconds, int precision = 2)
		{
			return UnitConverter.ToString (seconds, new string[] { "Hz", "Khz", "MHz" }, 1000, precision);
		}
	}
}

