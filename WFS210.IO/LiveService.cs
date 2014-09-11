using System;
using WFS210;
using System.ComponentModel.Design;

namespace WFS210.IO
{
	public class LiveService : Service
	{
		public TcpConnection Connection { get; private set; }

		public LiveService (Oscilloscope oscilloscope, TcpConnection connection)
			: base(oscilloscope)
		{
			this.Connection = connection;
		}

		public override void ApplySettings ()
		{
			Message message = new Message (Command.ModifySettings);

			message.Payload = new byte[10];
			message.Payload [0] = 0x00;

			Connection.Write (message);
		}

		public override void RequestSettings ()
		{
			Message message = new Message (Command.RequestSettings);

			Connection.Write (message);
		}

		public override void Update ()
		{
			// read incoming IO
		}
	}
}

