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
		/// Applies the settings.
		/// </summary>
		public override void ApplySettings ()
		{
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

		/// <summary>
		/// Updates both signals with the current oscilloscope settings. Should be
		/// called whenever you want to update the signals.
		/// </summary>
		public override void Update ()
		{
			foreach (Channel channel in Oscilloscope.Channels) {

				SignalGenerator.GenerateSignal (channel, true);
			}
		}


		public override bool Active {
			get {
				return true;
			}
		}
	}
}

