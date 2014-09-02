using System;

namespace WFS210.UI
{
	public class WFS210Tools
	{
		public WFS210Tools ()
		{
		}

		public string  GetTextFromVoltPerDivision (VoltsPerDivision vpd)
		{
			string result = "";
			switch (vpd) {
			case VoltsPerDivision.Vdiv100mV:
				result = "100mV/DIV";
				break;
			case VoltsPerDivision.Vdiv10mV:
				result = "10mV/DIV";
				break;
			case VoltsPerDivision.Vdiv10V:
				result = "10V/DIV";
				break;
			case VoltsPerDivision.Vdiv1V:
				result = "1V/DIV";
				break;
			case VoltsPerDivision.Vdiv200mV:
				result = "200mV/DIV";
				break;
			case VoltsPerDivision.Vdiv20mV:
				result = "20mV/DIV";
				break;
			case VoltsPerDivision.Vdiv20V:
				result = "20V/DIV";
				break;
			case VoltsPerDivision.Vdiv2V:
				result = "2V/DIV";
				break;
			case VoltsPerDivision.Vdiv4V:
				result = "4V/DIV";
				break;
			case VoltsPerDivision.Vdiv500mV:
				result = "500mV/DIV";
				break;
			case VoltsPerDivision.Vdiv50mV:
				result = "50mV/DIV";
				break;
			case VoltsPerDivision.Vdiv5mV:
				result = "5mV/DIV";
				break;
			case VoltsPerDivision.VdivNone:
				result = "Off";
				break;
			default:
				break;
			}
			return result;
		}

		public string  GetTextFromTimebase (TimeBase tb)
		{
			string result = "";
			switch (tb) {
			case TimeBase.Tdiv100ms:
				result = "100ms/DIV";
				break;
			case TimeBase.Tdiv100us:
				result = "100us/DIV";
				break;
			case TimeBase.Tdiv10ms:
				result = "10ms/DIV";
				break;
			case TimeBase.Tdiv10us:
				result = "10us/DIV";
				break;
			case TimeBase.Tdiv1ms:
				result = "1ms/DIV";
				break;
			case TimeBase.Tdiv1s:
				result = "1s/DIV";
				break;
			case TimeBase.Tdiv1us:
				result = "1us/DIV";
				break;
			case TimeBase.Tdiv200ms:
				result = "200ms/DIV";
				break;
			case TimeBase.Tdiv200us:
				result = "200us/DIV";
				break;
			case TimeBase.Tdiv20ms:
				result = "20ms/DIV";
				break;
			case TimeBase.Tdiv20us:
				result = "20us/DIV";
				break;
			case TimeBase.Tdiv2ms:
				result = "2ms/DIV";
				break;
			case TimeBase.Tdiv2us:
				result = "2us/DIV";
				break;
			case TimeBase.Tdiv500ms:
				result = "500ms/DIV";
				break;
			case TimeBase.Tdiv500us:
				result = "500us/DIV";
				break;
			case TimeBase.Tdiv50ms:
				result = "50ms/DIV";
				break;
			case TimeBase.Tdiv50us:
				result = "50us/DIV";
				break;
			case TimeBase.Tdiv5ms:
				result = "5ms/DIV";
				break;
			case TimeBase.Tdiv5us:
				result = "5us/DIV";
				break;
			default:
				break;
			}
			return result;
		}
	}
}

