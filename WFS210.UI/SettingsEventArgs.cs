using System;

namespace WFS210.UI
{
	public class SettingsEventArgs : EventArgs
	{
		public WifiSettings wifiSettings;
		public bool isDemo;
		public SettingsEventArgs (WifiSettings wifi,bool demo)
		{
			this.wifiSettings = wifi;
			this.isDemo = demo;
		}
	}
}

