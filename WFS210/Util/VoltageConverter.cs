using System;

namespace WFS210
{
	public static class VoltageConverter
	{
		public static string ToString(double voltage)
		{
			return UnitConverter.ToString (voltage, new string[] { "V", "mV" }, 1000);
		}
	}
}

