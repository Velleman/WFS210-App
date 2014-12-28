using System;
using System.Collections;
using Android.Graphics;

namespace aWFS210
{
	/// <summary>
	/// Trace.
	/// </summary>
	public class Trace : IEnumerable
	{
		/// <summary>
		/// The points that construct the trace.
		/// </summary>
		private Point[] points;

		/// <summary>
		/// Initializes a new instance of the <see cref="aWFS210.Trace"/> class
		/// that can hold Size number of points.
		/// </summary>
		/// <param name="size">Number of points.</param>
		public Trace (int size)
		{
			points = new Point[size];
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
		public Point this[int index]
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
		public Point[] ToPointArray()
		{
			return points;
		}

		/// <summary>
		/// Get an array of points that represents the trace.
		/// </summary>
		/// <returns>The point array.</returns>
		public float[] ToFloatArray(int Samples)
		{
			float[] floatArray;
			try{
				floatArray = new float[Samples * 2];
			int pointsCount = 0;
				for (int i = 0; i < floatArray.Length; i++) {
					floatArray [i] = points[pointsCount].X;
					i++;
					floatArray [i] = points[pointsCount].Y;
				pointsCount++;
			}
			}
			catch(Exception e) {
				floatArray = new float[1];
			}

			return floatArray;
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

