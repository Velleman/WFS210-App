using WFS210;
using WFS210.IO;

namespace WFS210.Services
{
	/// <summary>
	/// Live service.
	/// </summary>
	public class LiveService : Service
	{
		/// <summary>
		/// Gets the tcp connection used to communicate with the remote oscilloscope.
		/// </summary>
		/// <value>The connection.</value>
		public TcpConnection Connection { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.Services.LiveService"/> class.
		/// </summary>
		/// <param name="oscilloscope">Oscilloscope.</param>
		/// <param name="connection">Connection.</param>
		public LiveService (Oscilloscope oscilloscope, TcpConnection connection)
			: base (oscilloscope)
		{
			this.Connection = connection;
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
				DecodeSamplePacket (message.Payload);
				break;
			case Command.Settings:
				DecodeSettingsPacket (message.Payload);
				break;
			default:
				break;
			}
		}

		public void DecodeSamplePacket (byte[] payload)
		{
			throw new System.NotImplementedException ();
		}

		public void DecodeSettingsPacket (byte[] payload)
		{
			// Channel 1
			Oscilloscope.Channels [0].InputCoupling = (InputCoupling)payload [0];
			Oscilloscope.Channels [0].VoltsPerDivision = (VoltsPerDivision)payload [1];
			Oscilloscope.Channels [0].YPosition = payload [2];

			// Channel 2
			Oscilloscope.Channels [1].InputCoupling = (InputCoupling)payload [3];
			Oscilloscope.Channels [1].VoltsPerDivision = (VoltsPerDivision)payload [4];
			Oscilloscope.Channels [1].YPosition = payload [5];

			// Oscilloscope
			Oscilloscope.TimeBase = (TimeBase)payload [6];
			Oscilloscope.Trigger.Level = payload [7];
			DecodeTriggerSettings (payload [8]);
			DecodeModuleStatus (payload [9]);
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

