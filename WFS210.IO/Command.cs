using System;

namespace WFS210.IO
{
	/// <summary>
	/// The command class is a placeholder for all known command codes.
	/// </summary>
	public static class Command {

		/// <summary>
		/// Request WiFi settings.
		/// </summary>
		public static readonly byte RequestWifiSettings = 0x0a;

		/// <summary>
		/// Wifi settings.
		/// </summary>
		public static readonly byte WifiSettings = 0x22;

		/// <summary>
		/// Request device settings (Timebase etc).
		/// </summary>
		public static readonly byte RequestSettings = 0x10;

		/// <summary>
		/// Device settings.
		/// </summary>
		public static readonly byte Settings = 0x20;

		/// <summary>
		/// Modify device settings.
		/// </summary>
		public static readonly byte ModifySettings = 0x0b;

		/// <summary>
		/// Start the calibration process.
		/// </summary>
		public static readonly byte StartCalibration = 0xca;

		/// <summary>
		/// Request sample data. Samples are usually sent by the device
		/// automatically, so there is not much need to use this command.
		/// </summary>
		public static readonly byte RequestSampleData = 0x12;

		/// <summary>
		/// Sample data.
		/// </summary>
		public static readonly byte SampleData = 0x21;
	}
}

