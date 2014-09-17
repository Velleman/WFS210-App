using System;

namespace WFS210.Services
{
	public delegate void SettingsDelegate ();

	public abstract class Service
	{
		public Oscilloscope Oscilloscope { get; private set; }

		public event EventHandler SettingsChanged;

		public void OnSettingsChanged(EventArgs e)
		{
			if (SettingsChanged != null) 
			{
				SettingsChanged (this, e);
			}
		}

		protected Service (Oscilloscope oscilloscope)
		{
			this.Oscilloscope = oscilloscope;
		}

		public abstract void ApplySettings ();

		public abstract void RequestSettings ();

		/// <summary>
		/// Update this instance. This function should be called regularly.
		/// </summary>
		public abstract void Update ();
	}
}

