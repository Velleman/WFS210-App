using System;

namespace WFS210
{
	public static class WattConverter
	{
		public static string ToString(double voltage, int precision = 2)
		{
			return UnitConverter.ToString (voltage, new string[] { "W", "mW" }, 1000, precision);
		}
	}
}

