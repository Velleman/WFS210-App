using System;
using System.Drawing;
using System.Collections;

namespace WFS210.UI
{
	public class Trace : IEnumerable
	{
		PointF[] traceData;
		public Trace (int size)
		{
			traceData = new PointF[size];
		}

		/// <summary>
		/// Gets the total number of samples.
		/// </summary>
		/// <value>The total number of samples.</value>
		public int Count
		{
			get { return traceData.Length; }
		}

		/// <summary>
		/// Gets or sets the <see cref="iWFS210.Trace"/> at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public PointF this[int index]
		{
			get
			{
				return traceData [index];
			}
			set
			{
				traceData [index] = value;
			}
		}
			
		#region IEnumerable implementation
		public IEnumerator GetEnumerator ()
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

