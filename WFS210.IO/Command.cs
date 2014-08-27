using System;

namespace WFS210.IO
{
	/// <summary>
	/// The command class is a placeholder for all known command codes.
	/// </summary>
	public static class Command {

		public static readonly byte WifiSettingsRequest = 0x0a;
		public static readonly byte WifiSettings = 0x22;

		public static readonly byte SettingsRequest = 0x10;
		public static readonly byte Settings = 0x20;
		public static readonly byte ModifySettings = 0x0b;

		public static readonly byte StartCalibration = 0xca;

		public static readonly byte SampleDataRequest = 0x12;
		public static readonly byte SampleData = 0x21;
	}
}

