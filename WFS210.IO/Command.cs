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
		public const byte RequestWifiSettings = 0x0a;

		/// <summary>
		/// Wifi settings.
		/// </summary>
		public const byte WifiSettings = 0x22;

		/// <summary>
		/// Request device settings (Timebase etc).
		/// </summary>
		public const byte RequestSettings = 0x10;

		/// <summary>
		/// Device settings.
		/// </summary>
		public const byte Settings = 0x20;

		/// <summary>
		/// Modify device settings.
		/// </summary>
		public const byte ModifySettings = 0x11;

		/// <summary>
		/// Start the calibration process.
		/// </summary>
		public const byte StartCalibration = 0xca;

		/// <summary>
		/// Request sample data. Samples are usually sent by the device
		/// automatically, so there is not much need to use this command.
		/// </summary>
		public const byte RequestSampleData = 0x12;

		/// <summary>
		/// Sample data.
		/// </summary>
		public const byte SampleData = 0x21;
	}
}

