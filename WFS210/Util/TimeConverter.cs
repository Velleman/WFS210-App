using System;

namespace WFS210
{
	public class TimeConverter
	{
		public static string ToString(double seconds)
		{
			return UnitConverter.ToString (seconds, new string[] { "s", "ms", "us" }, 1000);
		}
	}
}

