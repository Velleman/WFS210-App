using System;

namespace WFS210.Util
{
	/// <summary>
	/// Range extension, provides utility functions.
	/// </summary>
	public static class RangeExtension
	{
		/// <summary>
		/// Limits a value to an absolute minimum and maximum.
		/// </summary>
		/// <returns>The limited value.</returns>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		public static int LimitToRange(this int value, int min, int max)
		{
			return (value < min
				? min
				: (value > max
					? max
					: value));
		}
	}
}

