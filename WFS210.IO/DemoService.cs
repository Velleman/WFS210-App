using System;

namespace WFS210.IO
{
	public class DemoService : Service
	{
		private SignalGenerator Generator { get; set; }

		public DemoService (Oscilloscope oscilloscope)
			: base(oscilloscope)
		{
			Generator = new SignalGenerator ();
		}

		public override void ApplySettings ()
		{
			RequestSettings ();
		}

		public override void RequestSettings ()
		{
			OnSettingsChanged (new EventArgs ());
		}

		public override void Update ()
		{
			for (int i = 0; i < Oscilloscope.Channels.Count; i++) {

				Generator.Generate (Oscilloscope, i);
			}
		}
	}
}

