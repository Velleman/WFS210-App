using System;

namespace WFS210
{
	public static class TimeBaseConverter
	{
		/// <summary>
		/// Time base lookup table. All values are in seconds (s).
		/// </summary>
		private static double[] timeBaseValues = new double[] {
			0.000001, 0.000002, 0.000005,
			0.00001, 0.00002, 0.00005,
			0.0001, 0.0002, 0.0005,
			0.001, 0.002, 0.005,
			0.01, 0.02, 0.05,
			0.1, 0.2, 0.5,
			1
		};

		public static double ToSeconds(TimeBase timeBase)
		{
			return timeBaseValues [(int)timeBase];
		}

		public static string ToString(TimeBase timeBase)
		{
			double seconds = TimeBaseConverter.ToSeconds (timeBase);

			return String.Format ("{0}/div", TimeConverter.ToString (seconds, 0));
		}
	}
}

