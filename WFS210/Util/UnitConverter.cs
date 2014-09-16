using System;

namespace WFS210
{
	public class UnitConverter
	{
		public static string ToString(double value, string[] units, double step)
		{
			int i = 0;

			while ((value < 1) && (i < units.Length)) {

				value *= step;
				i++;
			}

			return String.Format("{0}{1}", value, units[i]);
		}
	}
}

