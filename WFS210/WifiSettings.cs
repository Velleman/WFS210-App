using System;
using WFS210.Util;

namespace WFS210
{
	/// <summary>
	/// Wifi settings.
	/// </summary>
	public class WifiSettings
	{
		private int channel;

		/// <summary>
		/// Gets or sets the SSID.
		/// </summary>
		/// <value>The SSID.</value>
		public string SSID { get; set; }

		/// <summary>
		/// Gets or sets the wifi channel.
		/// </summary>
		/// <value>The wifi channel.</value>
		public int Channel
		{
			get { return this.channel; }
			set { this.channel = value.LimitToRange (1, 13); }
		}

		/// <summary>
		/// Gets or sets the wifi password.
		/// </summary>
		/// <value>The wifi password.</value>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets the Firmware version of the wifi module
		/// </summary>
		/// <value>The version.</value>
		public string Version { get; set; }
	}
}

