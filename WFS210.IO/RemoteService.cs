using System;
using WFS210;

namespace WFS210.IO
{
	public class RemoteService
	{
		public Oscilloscope Oscilloscope { get; private set; }

		public TcpConnection Connection { get; private set; }

		public RemoteService (Oscilloscope oscilloscope, TcpConnection connection)
		{
			this.Oscilloscope = oscilloscope;
			this.Connection = connection;
		}

		public void RequestSettings ()
		{
			Message message = new Message (Command.RequestSettings);

			Connection.Write (message);
		}
	}
}

