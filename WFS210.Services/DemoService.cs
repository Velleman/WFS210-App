using System;
using WFS210;

namespace WFS210.Services
{
	public class DemoService : Service
	{
		private SignalGenerator signalGenerator;

		/// <summary>
		/// Gets the generator used to generate a signal every time Update is called.
		/// </summary>
		/// <value>The signal generator.</value>
		public SignalGenerator SignalGenerator {
			get { return signalGenerator; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.Services.DemoService"/> class.
		/// </summary>
		/// <param name="oscilloscope">Oscilloscope.</param>
		public DemoService (Oscilloscope oscilloscope)
			: base(oscilloscope)
		{
			signalGenerator = new SignalGenerator ();
		}

		/// <summary>
		/// Activates this service.
		/// </summary>
		public override bool Activate ()
		{
			Update ();
			return true;
		}

		/// <summary>
		/// Dectivate this instance.
		/// </summary>
		public override void Deactivate ()
		{
		}

		/// <summary>
		/// Sends the wifi settings.
		/// </summary>
		public override void SendWifiSettings ()
		{

		}

		/// <summary>
		/// Request the wifi settings.
		/// </summary>
		public override void RequestWifiSettings ()
		{

		}

		/// <summary>
		/// Applies the settings.
		/// </summary>
		public override void ApplySettings ()
		{
			if (Oscilloscope.AutoRange) {
				Oscilloscope.Channels [0].InputCoupling = InputCoupling.AC;
				Oscilloscope.Channels [1].InputCoupling = InputCoupling.AC;
				Oscilloscope.Channels [0].VoltsPerDivision = VoltsPerDivision.Vdiv1V;
				Oscilloscope.Channels [1].VoltsPerDivision = VoltsPerDivision.Vdiv1V;
				Oscilloscope.TimeBase = TimeBase.Tdiv1ms;
			}
			RequestSettings ();
		}

		/// <summary>
		/// Requests the settings.
		/// </summary>
		public override void RequestSettings ()
		{
			OnSettingsChanged (new EventArgs ());
		}

		public override void RequestSamples ()
		{
			throw new NotImplementedException ();
		}

		public override void RequestCalibration ()
		{

		}

		/// <summary>
		/// Updates both signals with the current oscilloscope settings. Should be
		/// called whenever you want to update the signals.
		/// </summary>
		public override void Update ()
		{
			foreach (Channel channel in Oscilloscope.Channels) {

				SignalGenerator.GenerateSignal (channel, true);
				OnNewFullFrame (EventArgs.Empty);
			}
		}


		public override bool Active {
			get {
				return true;
			}
		}
	}
}

