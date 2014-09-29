using System;
using System.Collections;

namespace WFS210
{
	/// <summary>
	/// Sample buffer.
	/// </summary>
	public class SampleBuffer : IEnumerable
	{
		/// <summary>
		/// Internal sample storage.
		/// </summary>
		private readonly byte[] samples;

		/// <summary>
		/// Gets the total number of samples.
		/// </summary>
		/// <value>The total number of samples.</value>
		public int Count
		{
			get { return samples.Length; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.SampleBuffer"/> class
		/// that can hold the requested number of samples.
		/// </summary>
		/// <param name="size">.</param>
		public SampleBuffer(int size) {

			samples = new byte[size];
		}

		/// <summary>
		/// Gets or sets the specified sample.
		/// </summary>
		/// <param name="sample">Index of the sample.</param>
		public byte this[int sample]
		{
			get
			{
				return samples [sample];
			}
			set
			{
				samples [sample] = value;
			}
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator GetEnumerator ()
		{
			return samples.GetEnumerator ();
		}
	}
}

