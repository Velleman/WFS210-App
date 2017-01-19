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
			return Math.Min (Math.Max (value, min), max);
		}

		/// <summary>
		/// Cycles an enum a specified number of values forwards or backwards.
		/// </summary>
		/// <param name="value">Enum object to cycle through.</param>
		/// <param name="amount">Number of positions to cycle (can be negative).</param>
		/// <typeparam name="T">The resulting enum value.</typeparam>
		public static T Cycle<T>(this T value, int amount)
		{
			T[] values = (T[])Enum.GetValues (value.GetType ());
			int index = Array.IndexOf<T> (values, value) + amount;
			return values [index.LimitToRange (0, values.Length - 1)];
		}
	}
}

