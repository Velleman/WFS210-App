using System;

namespace WFS210
{
	public class VoltageConverter
	{
		public static string ToString(double voltage)
		{
			return UnitConverter.ToString (voltage, new string[] { "V", "mV" }, 1000);
		}
	}
}

