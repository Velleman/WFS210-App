using System;

namespace WFS210.Services
{
	public delegate void SettingsDelegate ();

	public abstract class Service
	{
		private readonly Oscilloscope oscilloscope;

		/// <summary>
		/// Gets the oscilloscope associated with this service.
		/// </summary>
		/// <value>The oscilloscope.</value>
		public Oscilloscope Oscilloscope {
			get { return oscilloscope; }
		}
		/// <summary>
		/// Gets a value indicating whether this <see cref="WFS210.Services.Service"/> is active.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public abstract bool Active {
			get;
		}

		/// <summary>
		/// Occurs when settings have changed.
		/// </summary>
		public event EventHandler SettingsChanged;

		/// <summary>
		/// Raises the settings changed event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		public void OnSettingsChanged(EventArgs e)
		{
			if (SettingsChanged != null) 
			{
				SettingsChanged (this, e);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.Services.Service"/> class.
		/// </summary>
		/// <param name="oscilloscope">Oscilloscope.</param>
		protected Service (Oscilloscope oscilloscope)
		{
			this.oscilloscope = oscilloscope;
		}
		/// <summary>
		/// Activate this instance.
		/// </summary>
		public abstract bool Activate ();

		/// <summary>
		/// Deactivate this instance.
		/// </summary>
		public abstract void Deactivate ();

		/// <summary>
		/// Applies the settings.
		/// </summary>
		public abstract void ApplySettings ();

		/// <summary>
		/// Requests the settings.
		/// </summary>
		public abstract void RequestSettings ();

		/// <summary>
		/// Requests Samples.
		/// </summary>
		public abstract void RequestSamples ();

		public abstract void RequestCalibration ();

		/// <summary>
		/// Update this instance. This function should be called regularly.
		/// </summary>
		public abstract void Update ();

		public void Execute(ServiceCommand command)
		{
			command.Execute (this);
			ApplySettings ();
		}
	}
}

