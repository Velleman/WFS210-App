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
			: base(oscilloscope)
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
		}
	}
}

