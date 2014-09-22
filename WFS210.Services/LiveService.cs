using WFS210;
using WFS210.IO;

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
		public override void Activate ()
		{
			// Automatically connect
		}

		/// <summary>
		/// Applies the settings.
		/// </summary>
		public override void ApplySettings ()
		{
			var message = new Message (Command.ModifySettings);

			message.Payload = new byte[10];
			message.Payload [0] = 0x00;

			Connection.Write (message);
		}

		/// <summary>
		/// Requests the settings.
		/// </summary>
		public override void RequestSettings ()
		{
			var message = new Message (Command.RequestSettings);

			Connection.Write (message);
		}

		/// <summary>
		/// Performs basic IO. Should be called often.
		/// </summary>
		public override void Update ()
		{
			var message = Connection.Read ();

			switch (message.Command) {
			case Command.SampleData:
				DecodeSamplesMessage (message);
				break;
			case Command.Settings:
				DecodeSettingsMessage (message);
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// Decodes and stores the samples from a samples message.
		/// </summary>
		/// <param name="message">Samples message.</param>
		public void DecodeSamplesMessage (Message message)
		{
			throw new System.NotImplementedException ();
		}

		/// <summary>
		/// Decodes and applies the settings in a settings packet.
		/// </summary>
		/// <param name="message">Settings message.</param>
		public void DecodeSettingsMessage (Message message)
		{
			// Channel 1
			Oscilloscope.Channels [0].InputCoupling = (InputCoupling)message.Payload [0];
			Oscilloscope.Channels [0].VoltsPerDivision = (VoltsPerDivision)message.Payload [1];
			Oscilloscope.Channels [0].YPosition = message.Payload [2];

			// Channel 2
			Oscilloscope.Channels [1].InputCoupling = (InputCoupling)message.Payload [3];
			Oscilloscope.Channels [1].VoltsPerDivision = (VoltsPerDivision)message.Payload [4];
			Oscilloscope.Channels [1].YPosition = message.Payload [5];

			// Oscilloscope
			Oscilloscope.TimeBase = (TimeBase)message.Payload [6];
			Oscilloscope.Trigger.Level = message.Payload [7];
			DecodeTriggerSettings (message.Payload [8]);
			DecodeModuleStatus (message.Payload [9]);
		}

		protected void DecodeTriggerSettings (byte flags)
		{
			Oscilloscope.Trigger.Mode = (TriggerMode)(flags & 3);
			Oscilloscope.Trigger.Slope = (TriggerSlope)((flags & 4) >> 2);
			Oscilloscope.Trigger.Channel = ((flags & 8) >> 3);
			Oscilloscope.Hold = (((flags & 16) >> 4) == 1);
			Oscilloscope.AutoRange = (((flags & 128) >> 7) == 1);
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

