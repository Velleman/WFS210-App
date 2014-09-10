using System;

namespace WFS210.IO
{
	public delegate void SettingsDelegate ();

	public abstract class Service
	{
		public Oscilloscope Oscilloscope { get; private set; }

		public TcpConnection Connection { get; private set; }

		public SettingsDelegate SettingsChanged;

		public Service (Oscilloscope oscilloscope, TcpConnection connection)
		{
			this.Oscilloscope = oscilloscope;
			this.Connection = connection;
		}

		public abstract void ApplySettings ();

		public abstract void RequestSettings ();

		/// <summary>
		/// Update this instance. This function should be called regularly.
		/// </summary>
		public abstract void Update ();
	}
}

