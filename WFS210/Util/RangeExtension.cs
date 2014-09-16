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

		/// <summary>
		/// Cycles an enum a specified number of values forwards or backwards.
		/// </summary>
		/// <param name="src">Enum object to cycle through.</param>
		/// <param name="value">Number of positions to cycle (can be negative).</param>
		/// <typeparam name="T">The resulting enum value.</typeparam>
		public static T Cycle<T>(this T src, int value)
		{
			T[] values = (T[])Enum.GetValues (src.GetType ());
			int index = Array.IndexOf<T> (values, src) + value;
			return values [index.LimitToRange (0, values.Length - 1)];
		}
	}
}

