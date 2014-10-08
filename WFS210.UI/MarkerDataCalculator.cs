using System;
using System.Drawing;

namespace WFS210.UI
{
	static class MarkerDataCalculator
	{
		/// <summary>
		/// Calculates the time.
		/// </summary>
		/// <returns>The time.</returns>
		/// <param name="timebase">Timebase.</param>
		/// <param name="marker1">Marker1.</param>
		/// <param name="marker2">Marker2.</param>
		/// <param name="frame">Frame.</param>
		public static double CalculateTime(TimeBase timebase,Marker marker1,Marker marker2,RectangleF frame)
		{
			var pixelsPerDiv = frame.Width / 15;
			var distance = Math.Abs (marker1.Value - marker2.Value);
			var divs = distance / pixelsPerDiv;
			var result = divs * TimeBaseConverter.ToSeconds(timebase);
			return result;
		}


		/// <summary>
		/// Calculates the frequency.
		/// </summary>
		/// <returns>The frequency.</returns>
		/// <param name="timebase">Timebase.</param>
		/// <param name="marker1">Marker1.</param>
		/// <param name="marker2">Marker2.</param>
		/// <param name="frame">Frame.</param>
		public static double CalculateFrequency(TimeBase timebase,Marker marker1,Marker marker2,RectangleF frame)
		{
			var time =CalculateTime (timebase, marker1, marker2, frame);
			var frequency = 1 / Math.Round(time,9);
			return frequency;
		}

		/// <summary>
		/// Calculates the voltage between 2 markers.
		/// </summary>
		/// <returns>The D.</returns>
		/// <param name="vpd">Vpd.</param>
		/// <param name="marker1">Marker1.</param>
		/// <param name="marker2">Marker2.</param>
		/// <param name="frame">Frame.</param>
		public static double CalculateDV(VoltsPerDivision vpd,AttenuationFactor attenuationFactor,Marker marker1, Marker marker2,RectangleF frame)
		{

			var pixelsPerDiv = frame.Height / 10 ;
			var distance = Math.Abs (marker1.Value - marker2.Value);
			var divs = distance / pixelsPerDiv;
			var result = divs * VoltsPerDivisionConverter.ToVolts(vpd,attenuationFactor);
			return result;
		}
	}
}

