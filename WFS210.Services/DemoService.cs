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

		/// <summary>
		/// Updates both signals with the current oscilloscope settings. Should be
		/// called whenever you want to update the signals.
		/// </summary>
		public override void Update ()
		{
			for (int i = 0; i < Oscilloscope.Channels.Count; i++) {

				SignalGenerator.GenerateSignal (Oscilloscope, i);
			}
		}
	}
}

