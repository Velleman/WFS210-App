using System;

using WFS210.Util;

namespace WFS210
{
	/// <summary>
	/// Channel.
	/// </summary>
	public class Channel
	{
		/// <summary>
		/// Gets the oscilloscope's capabilities.
		/// </summary>
		/// <value>The oscilloscope's capabilities.</value>
		public DeviceContext DeviceContext { get; private set; }

		private int yPosition;

		/// <summary>
		/// Gets or sets the number of volts per division.
		/// </summary>
		/// <value>The number of volts per division.</value>
		public VoltsPerDivision VoltsPerDivision { get; set; }

		/// <summary>
		/// Gets or sets the probe's attenuation factor.
		/// </summary>
		/// <value>The probe's attenuation factor.</value>
		public AttenuationFactor AttenuationFactor { get; set; }

		/// <summary>
		/// Selects the input coupling (AC, DC, ground).
		/// </summary>
		/// <value>The input coupling.</value>
		public InputCoupling InputCoupling { get; set; }

		/// <summary>
		/// The sample buffer that contains all sampled values.
		/// </summary>
		/// <value>The sample buffer.</value>
		public SampleBuffer Samples { get; private set; }

		/// <summary>
		/// Gets or sets the Y position of the signal. This value is limited to
		/// the minimum and maximum sample value supported by the oscilloscope.
		/// </summary>
		/// <value>The Y position.</value>
		public int YPosition
		{
			get { return yPosition; }
			set
			{
				yPosition = value.LimitToRange(
					DeviceContext.SampleMin, DeviceContext.SampleMax);
			}
		}

		/// <summary>
		/// Creates a new channel with default settings and allocates
		/// a sample buffer of the requested size.
		/// </summary>
		/// <param name="sampleBufferSize">Required sample buffer size.</param>
		/// <param name="context">Device context.</param>
		public Channel (int sampleBufferSize, DeviceContext context)
		{
			DeviceContext = context;
			Samples = new SampleBuffer(sampleBufferSize);
			VoltsPerDivision = VoltsPerDivision.Vdiv1V;
			InputCoupling = InputCoupling.AC;
			AttenuationFactor = AttenuationFactor.X1;
			YPosition = DeviceContext.CenterPosition;
		}

		/// <summary>
		/// Calculates the number of volts per unit for the current
		/// volts per division and probe attenuation settings.
		/// </summary>
		/// <returns>The number of volts per unit.</returns>
		public double VoltsPerUnit() {

			double voltsPerUnit = (VoltsPerDivisionConverter.ToVolts(VoltsPerDivision) / DeviceContext.UnitsPerDivision);

			if (AttenuationFactor == AttenuationFactor.X10) {
				voltsPerUnit *= 10;
			}

			return voltsPerUnit;
		}

		/// <summary>
		/// Gets a sample's value in volts (V).
		/// </summary>
		/// <param name="sample">The sample's value in volts (V).</param>
		public double Voltage(int sample)
		{
			return (sample * VoltsPerUnit());
		}

		/// <summary>
		/// Calculates the average DC voltage of all samples.
		/// </summary>
		public double Vdc()
		{
			double value = 0;

			foreach(byte sample in Samples) {
				value += Voltage (sample);
			}

			return (value / Samples.Count);
		}

		/// <summary>
		/// Calculates the minimum voltage of all samples.
		/// </summary>
		public double Vmin()
		{
			byte minValue = 0;

			for (int i = 0; i < Samples.Count; i++) {

				if ((i == 0) || (Samples [i] < minValue)) {

					minValue = Samples [i];
				}
			}

			return Voltage(minValue);
		}

		/// <summary>
		/// Calculates the maximum voltage of all samples.
		/// </summary>
		public double Vmax()
		{
			byte maxValue = 0;

			for (int i = 0; i < Samples.Count; i++) {

				if ((i == 0) || (Samples [i] > maxValue)) {

					maxValue = Samples [i];
				}
			}

			return Voltage(maxValue);
		}
	}
}

