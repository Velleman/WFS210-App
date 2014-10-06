using WFS210;
using WFS210.IO;
using System;

namespace WFS210.Services
{
	/// <summary>
	/// Live service.
	/// </summary>
	public class LiveService : Service
	{
		private readonly TcpConnection connection;

		/// <summary>
		/// Gets the tcp connection used to communicate with the remote oscilloscope.
		/// </summary>
		/// <value>The connection.</value>
		public TcpConnection Connection {
			get { return connection; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.Services.LiveService"/> class.
		/// </summary>
		/// <param name="oscilloscope">Oscilloscope.</param>
		public LiveService (Oscilloscope oscilloscope)
			: base (oscilloscope)
		{
			this.connection = new TcpConnection ();
		}

		/// <summary>
		/// Activate this service by establishing a connection to the device.
		/// </summary>
		public override bool Activate ()
		{
			return this.connection.Connect ();
		}

		/// <summary>
		/// Deactivate this instance.
		/// </summary>
		public override void Deactivate ()
		{
			this.connection.Close ();
		}

		public override bool Active {
			get {
				return this.connection.Connected;
			}
		}

		/// <summary>
		/// Applies the settings.
		/// </summary>
		public override void ApplySettings ()
		{
			var message = new Message (Command.ModifySettings);

			message.Payload = new byte[12];

			//Channel 1
			message.Payload [0] = 0;
			message.Payload [1] = 0;
			message.Payload [2] = (byte)Oscilloscope.Channels [0].InputCoupling;
			message.Payload [3] = (byte)Oscilloscope.Channels [0].VoltsPerDivision;
			message.Payload [4] = (byte)Oscilloscope.Channels [0].YPosition;

			//Channel 2
			message.Payload [5] = (byte)Oscilloscope.Channels [1].InputCoupling;
			message.Payload [6] = (byte)Oscilloscope.Channels [1].VoltsPerDivision;
			message.Payload [7] = (byte)Oscilloscope.Channels [1].YPosition;

			//Oscilloscope
			message.Payload [8] = (byte)Oscilloscope.TimeBase;
			message.Payload [9] = (byte)Oscilloscope.Trigger.Level;
			message.Payload [10] = EncodeTriggerSettings ();
			message.Payload [11] = 0;

			Connection.Write (message);
		}

		/// <summary>
		/// Requests the settings.
		/// </summary>
		public override void RequestSettings ()
		{
			var message = new Message (Command.RequestSettings);
			message.Payload = new byte[2];
			Connection.Write (message);
		}

		/// <summary>
		/// Requests the Samples.
		/// </summary>
		public override void RequestSamples ()
		{
			var message = new Message (Command.RequestSampleData);
			message.Payload = new byte[2];
			Connection.Write (message);
		}

		/// <summary>
		/// Requests the calibration.
		/// </summary>
		public override void RequestCalibration ()
		{
			var message = new Message (Command.StartCalibration);
			message.Payload = new byte[2];
			Connection.Write (message);
		}

		public override void RequestWifiSettings ()
		{
			var message = new Message (Command.RequestWifiSettings);
			message.Payload = new byte[2];
			connection.Write (message);
		}

		/// <summary>
		/// Sends the wifi settings.
		/// </summary>
		public override void SendWifiSettings ()
		{
			var message = new Message (Command.SendWifiSettings);
			message.Payload = new byte[68];
			EncodeWifiSettings (message);
			Connection.Write (message);
		}

		/// <summary>
		/// Performs basic IO. Should be called often.
		/// </summary>
		public override void Update ()
		{
			var message = Connection.Read ();

			while (message != null) {
				switch (message.Command) {
				case Command.SampleData:
					DecodeSamplesMessage (message);
					break;
				case Command.Settings:
					DecodeSettingsMessage (message);
					OnSettingsChanged (null);
					break;
				case Command.WifiSettings:
					DecodeWifiSettingsMessage (message);
					break;
				default:
					break;
				}
				message = Connection.Read ();
			}
		}

		/// <summary>
		/// Decodes and stores the samples from a samples message.
		/// </summary>
		/// <param name="message">Samples message.</param>
		public void DecodeSamplesMessage (Message message)
		{
			var payload = message.Payload;
			int offSet = payload [1] * 256 + payload [0];
			var bufferSize = (payload.Length - 12) / 2;


			var position = 12;
			for (int i = 0; i < bufferSize; i++) {
				Oscilloscope.Channels [0].Samples [i + offSet / 2] = payload [position];
				position++;
				Oscilloscope.Channels [1].Samples [i + offSet / 2] = payload [position];
				position++;
			}
		}

		/// <summary>
		/// Encodes the wifi settings.
		/// </summary>
		/// <param name="message">Message.</param>
		void EncodeWifiSettings (Message message)
		{
			var payload = message.Payload;
			payload [0] = 0;
			payload [1] = 0;
			var channel = BitConverter.GetBytes (Oscilloscope.WifiSetting.Channel);
			payload [2] = channel [0];
			payload [3] = channel [1];

			var data = new char[32];
			var wifiname = Oscilloscope.WifiSetting.SSID.ToCharArray ();
			wifiname.CopyTo (data, 0);
			for (int i = 4; i < 36; i++) {
				payload [i] = (byte)data [i - 4];
			}

			var password = Oscilloscope.WifiSetting.Password.ToCharArray ();
			wifiname.CopyTo (data, 0);
			for (int i = 37; i < 68; i++) {
				payload [i] = (byte)data [i - 37];
			}
		}

		/// <summary>
		/// Decodes the wifi settings message.
		/// </summary>
		/// <param name="message">Message.</param>
		void DecodeWifiSettingsMessage (Message message)
		{
			var payload = message.Payload;
			var channel = new byte[]{ payload [2], payload [3] };
			Oscilloscope.WifiSetting.Channel = (int)BitConverter.ToInt16 (channel, 0);

			var SSID = new char[32];
			int i = 4;
			do {
				SSID [i - 4] = (char)payload [i];
				i++;
			} while(payload [i] != 0);
			var SSIDString = new string (SSID);
			var SSIDtrimmed = SSIDString.Split (new char[1]{ '\0' }, 2) [0];
			Oscilloscope.WifiSetting.SSID = SSIDtrimmed;

			var Password = new char[32];
			i = 37;
			do {
				Password [i - 37] = (char)payload [i];
				i++;
			} while(payload [i] != 0);
			var PasswordString = new string (Password);
			var Passwordtrimmed = PasswordString.Split (new char[1]{ '\0' }, 2) [0];
			Oscilloscope.WifiSetting.Password = Passwordtrimmed;

			//TODO recover Build number and Version

		}

		/// <summary>
		/// Decodes and applies the settings in a settings packet.
		/// </summary>
		/// <param name="message">Settings message.</param>
		public void DecodeSettingsMessage (Message message)
		{
			// Channel 1
			Oscilloscope.Channels [0].InputCoupling = (InputCoupling)message.Payload [2];
			Oscilloscope.Channels [0].VoltsPerDivision = (VoltsPerDivision)message.Payload [3];
			Oscilloscope.Channels [0].YPosition = message.Payload [4];

			// Channel 2
			Oscilloscope.Channels [1].InputCoupling = (InputCoupling)message.Payload [5];
			Oscilloscope.Channels [1].VoltsPerDivision = (VoltsPerDivision)message.Payload [6];
			Oscilloscope.Channels [1].YPosition = message.Payload [7];

			// Oscilloscope
			Oscilloscope.TimeBase = (TimeBase)message.Payload [8];
			Oscilloscope.Trigger.Level = message.Payload [9];
			DecodeTriggerSettings (message.Payload [10]);
			DecodeModuleStatus (message.Payload [11]);
		}

		byte EncodeTriggerSettings ()
		{
			byte flags = 0x00;
			flags |= (byte)((Oscilloscope.AutoRange ? 1 : 0) << 7);
			flags |= (byte)((Oscilloscope.Hold ? 1 : 0) << 4);
			flags |= (byte)((Oscilloscope.Trigger.Channel) << 3);
			flags |= (byte)((int)(Oscilloscope.Trigger.Slope) << 2);
			flags |= (byte)Oscilloscope.Trigger.Mode;
			return flags;
		}

		protected void DecodeTriggerSettings (byte flags)
		{
			Oscilloscope.Trigger.Mode = (TriggerMode)(flags & 3);
			Oscilloscope.Trigger.Slope = (TriggerSlope)((flags & 4) >> 2);
			Oscilloscope.Trigger.Channel = ((flags & 8) >> 3);
			Oscilloscope.Hold = (((flags & 16) >> 4) == 1);
			Oscilloscope.AutoRange = (((flags & 128) >> 7) == 1);
		}

		byte EncodeModuleStatus ()
		{
			byte flags = 0x00;
			switch (Oscilloscope.BatteryStatus) {
			case BatteryStatus.BatteryLow:
				flags |= 0x03;
				break;
			case BatteryStatus.Charged:
				flags |= 0x05;
				break;
			case BatteryStatus.Charging:
				flags |= 0x04;
				break;
			default:
				break;
			}
			flags |= (byte)(Oscilloscope.Calibrating ? 1 : 0 << 4);
			return flags;
		}

		protected void DecodeModuleStatus (byte flags)
		{
			int batteryStatus = (flags & 0x07);

			switch (batteryStatus) {
			case 0x04:
				Oscilloscope.BatteryStatus = BatteryStatus.Charging;
				break;
			case 0x05:
				Oscilloscope.BatteryStatus = BatteryStatus.Charged;
				break;
			case 0x03:
				Oscilloscope.BatteryStatus = BatteryStatus.BatteryLow;
				break;
			}

			Oscilloscope.Calibrating = (((flags & 16) >> 4) == 1);
		}
	}
}

