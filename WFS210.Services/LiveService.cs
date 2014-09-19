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
			Message message = new Message (Command.ModifySettings);

			message.Payload = new byte[10];
			message.Payload [0] = 0x00;

			Connection.Write (message);
		}

		/// <summary>
		/// Requests the settings.
		/// </summary>
		public override void RequestSettings ()
		{
			Message message = new Message (Command.RequestSettings);

			Connection.Write (message);
		}

		/// <summary>
		/// Performs basic IO. Should be called often.
		/// </summary>
		public override void Update ()
		{
			// read incoming IO
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

		void DecodeSamplePacket (byte[] payload)
		{
			throw new System.NotImplementedException ();
		}

		void DecodeSettingsPacket (byte[] payload)
		{
			Oscilloscope.Channels [0].InputCoupling = (InputCoupling)payload [0];
			Oscilloscope.Channels [0].VoltsPerDivision = (VoltsPerDivision)payload [1];
			Oscilloscope.Channels [0].YPosition = payload [2];
			Oscilloscope.Channels [1].InputCoupling = (InputCoupling)payload [3];
			Oscilloscope.Channels [1].VoltsPerDivision = (VoltsPerDivision)payload [4];
			Oscilloscope.Channels [1].YPosition = payload [5];
			Oscilloscope.TimeBase = (TimeBase)payload [6];
			Oscilloscope.Trigger.Level = payload [7];
			DecodeTriggerSettings (payload [8]);
			DecodeModuleStatus (payload [9]);
		}

		void DecodeTriggerSettings (byte triggerSettings)
		{
			Oscilloscope.Trigger.Mode = (TriggerMode)(triggerSettings & 3);
			Oscilloscope.Trigger.Slope = (TriggerSlope)((triggerSettings & 4) >> 2);
			Oscilloscope.Trigger.Channel = ((triggerSettings & 8) >> 3);
			var hold = (triggerSettings & 16) >> 4;
			if (hold == 1)
				Oscilloscope.Hold = true;
			else
				Oscilloscope.Hold = false;
			var autoRange = ((triggerSettings & 128) >> 7);
			if (autoRange == 1)
				Oscilloscope.AutoRange = true;
			else
				Oscilloscope.AutoRange = false;
		}

		void DecodeModuleStatus (byte b)
		{
			var batStatus = b & 7;
			if (batStatus == 4)
				Oscilloscope.BatteryStatus = BatteryStatus.Charging;
			if (batStatus == 5)
				Oscilloscope.BatteryStatus = BatteryStatus.Charged;
			if (batStatus == 3)
				Oscilloscope.BatteryStatus = BatteryStatus.BatteryLow;
			var calibrating = ((b & 16) >> 4);
			if (calibrating == 1)
				Oscilloscope.Calibrating = true;
			else
				Oscilloscope.Calibrating = false;
		}
	}
}

