using System;
using System.Drawing;

namespace WFS210
{
	public class MarkerDataCalculator
	{
		public MarkerDataCalculator ()
		{
		}

		public static double CalculateTime(TimeBase timebase,float position1,float position2,DeviceContext context,RectangleF frame)
		{
			var pixelsPerDiv = frame.Width / 15;
			var distance = Math.Abs (position1 - position2);
			var divs = distance / pixelsPerDiv;
			var result = divs * GetValueFromTimeBase(timebase);
			return result;
		}

		public static double CalculateFrequency(TimeBase timebase,float position1,float position2,DeviceContext context,RectangleF frame)
		{
			var time =CalculateTime (timebase, position1, position2, context, frame);
			var frequency = 1 / Math.Round(time,9);
			return frequency;
		}

		public static double CalculateDV(VoltsPerDivision vpd,float position1, float position2,DeviceContext context,RectangleF frame)
		{
			var pixelsPerDiv = frame.Height / 10 ;
			var distance = Math.Abs (position1 - position2);
			var divs = distance / pixelsPerDiv;
			var result = divs * GetValueFromVoltPerDivision(vpd);
			return result;
		}

		public static double CalculateVdc()
		{
			return 0;
		}

		public static double CalculateTrms()
		{
			return 0;
		}

		public static double CalculateRms()
		{
			return 0;
		}

		public static double CalculateVmax()
		{
			return 0;
		}

		public static double CalculateVmin()
		{
			return 0;
		}

		private static double GetValueFromVoltPerDivision(VoltsPerDivision vpd)
		{
			double result = 0;
			switch (vpd) {
			case VoltsPerDivision.Vdiv100mV:
				result = 0.1;
				break;
			case VoltsPerDivision.Vdiv10mV:
				result = 0.01;
				break;
			case VoltsPerDivision.Vdiv10V:
				result = 10;
				break;
			case VoltsPerDivision.Vdiv1V:
				result = 1;
				break;
			case VoltsPerDivision.Vdiv200mV:
				result = 0.2;
				break;
			case VoltsPerDivision.Vdiv20mV:
				result = 0.02;
				break;
			case VoltsPerDivision.Vdiv20V:
				result = 20;
				break;
			case VoltsPerDivision.Vdiv2V:
				result = 2;
				break;
			case VoltsPerDivision.Vdiv4V:
				result = 4;
				break;
			case VoltsPerDivision.Vdiv500mV:
				result = 0.5;
				break;
			case VoltsPerDivision.Vdiv50mV:
				result = 0.05;
				break;
			case VoltsPerDivision.Vdiv5mV:
				result = 0.005;
				break;
			case VoltsPerDivision.VdivNone:
				result = 0;
				break;
			default:
				break;
			}
			return result;
		}

		private static double GetValueFromTimeBase(TimeBase timebase)
		{
			double result = 0;
			switch (timebase) {
			case TimeBase.Tdiv100ms:
				result = 0.1d;;
				break;
			case TimeBase.Tdiv100us:
				result = 0.0001d;
				break;
			case TimeBase.Tdiv10ms:
				result = 0.01d;
				break;
			case TimeBase.Tdiv10us:
				result = 0.00001d;
				break;
			case TimeBase.Tdiv1ms:
				result = 0.001d;
				break;
			case TimeBase.Tdiv1s:
				result = 1d;
				break;
			case TimeBase.Tdiv1us:
				result = 0.000001d;
				break;
			case TimeBase.Tdiv200ms:
				result = 0.2d;
				break;
			case TimeBase.Tdiv200us:
				result = 0.0002d;
				break;
			case TimeBase.Tdiv20ms:
				result = 0.02d;
				break;
			case TimeBase.Tdiv20us:
				result = 0.000020d;
				break;
			case TimeBase.Tdiv2ms:
				result = 0.002d;
				break;
			case TimeBase.Tdiv2us:
				result = 0.000002d;
				break;
			case TimeBase.Tdiv500ms:
				result = 0.5d;
				break;
			case TimeBase.Tdiv500us:
				result = 0.000500d;
				break;
			case TimeBase.Tdiv50ms:
				result = 0.050d;
				break;
			case TimeBase.Tdiv50us:
				result = 0.000050d;
				break;
			case TimeBase.Tdiv5ms:
				result = 0.005d;
				break;
			case TimeBase.Tdiv5us:
				result = 0.000005d;
				break;
			default:
				break;
			}
			return result;
		}
	}
}

