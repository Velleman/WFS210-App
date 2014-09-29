using System;

namespace WFS210.UI
{
	public class SettingsEventArgs : EventArgs
	{
		/// <summary>
		/// The wifi settings.
		/// </summary>
		public WifiSettings WifiSettings;

		/// <summary>
		/// is demo running or nut.
		/// </summary>
		public bool isDemo;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.SettingsEventArgs"/> class.
		/// </summary>
		/// <param name="wifi">Wifi.</param>
		/// <param name="demo">If set to <c>true</c> demo.</param>
		public SettingsEventArgs (WifiSettings wifi,bool demo)
		{
			this.WifiSettings = wifi;
			this.isDemo = demo;
		}
	}
}

