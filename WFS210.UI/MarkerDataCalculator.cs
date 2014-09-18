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
			var result = divs * TimeBaseConverter.ToSeconds(timebase);
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
			var result = divs * VoltsPerDivisionConverter.ToVolts(vpd);
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
	}
}

