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

		private DeviceContext context;

		/// <summary>
		/// Gets or sets a value indicating whether there is a overflow in the samples.
		/// </summary>
		/// <value><c>true</c> if there is a overflow; otherwise, <c>false</c>.</value>
		public bool Overflow { get; set; }

		/// <summary>
		/// Gets the latest point in the buffer
		/// </summary>
		/// <value>The latest point that is added to the buffer</value>
		public int LatestPoint { get; private set;}

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
		public SampleBuffer(int size,DeviceContext context) {

			samples = new byte[size];
			this.context = context;
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
				if (value > context.SampleMax || value < context.SampleMin) {
					if (!Overflow)
						Overflow = true;
				}
				if (sample > LatestPoint) {
					if (sample != 4095)
					LatestPoint = sample;
				}
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

		/// <summary>
		/// Clears the data.
		/// </summary>
		public void Clear ()
		{
			for(int i = 0; i< samples.Length; i ++)
			{
				samples[i] = 0;
			}
			LatestPoint = 0;
		}
	}
}

