using System;

namespace WFS210
{
	public static class UnitConverter
	{
		public static string ToString(double value, string[] units, double step)
		{
			int i = 0;

			while ((Math.Abs(value) < 1) && (i < units.Length)) {

				value *= step;
				i++;
			}

			return String.Format("{0:N2}{1}", value, units[i]);
		}
	}
}

