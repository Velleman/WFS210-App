using System;
using WFS210.Util;

namespace WFS210
{
	public class WifiSettings
	{
		public string Name { get; set; }

		private int channel;

		public string Password { get; set;}

		public string Version { get; set;}

		public WifiSettings ()
		{
		}

		public int Channel {
			get{ return this.channel; }
			set{ this.channel = RangeExtension.LimitToRange (value, 1, 13); }
		}


	}
}

