using System;

namespace WFS210
{
	public static class DecibelConverter
	{
		public static string ToString(double voltage, int precision = 2)
		{
			return UnitConverter.ToString (voltage, new string[] { "dB", "mdB" }, 1000, precision);
		}
	}
}

