using System;
using System.Drawing;
using System.Collections;

namespace WFS210.UI
{
	/// <summary>
	/// Trace.
	/// </summary>
	public class Trace : IEnumerable
	{
		/// <summary>
		/// The points that construct the trace.
		/// </summary>
		private PointF[] points;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.Trace"/> class
		/// that can hold Size number of points.
		/// </summary>
		/// <param name="size">Number of points.</param>
		public Trace (int size)
		{
			points = new PointF[size];
		}

		/// <summary>
		/// Gets the total number of points in the trace.
		/// </summary>
		/// <value>The total number of points.</value>
		public int Count
		{
			get { return points.Length; }
		}

		/// <summary>
		/// Gets or sets a specified point in the trace.
		/// </summary>
		/// <param name="index">Index of the point to get/set.</param>
		public PointF this[int index]
		{
			get
			{
				return points [index];
			}
			set
			{
				points [index] = value;
			}
		}

		/// <summary>
		/// Get an array of points that represents the trace.
		/// </summary>
		/// <returns>The point array.</returns>
		public PointF[] ToPointArray()
		{
			return points;
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator GetEnumerator ()
		{
			return points.GetEnumerator ();
		}
	}
}

