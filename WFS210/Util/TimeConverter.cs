using System;

namespace WFS210
{
	public static class TimeConverter
	{
		public static string ToString(double seconds, int precision = 0)
		{
			return UnitConverter.ToString (seconds, new string[] { "s", "ms", "us" }, 1000, precision);
		}
	}
}

