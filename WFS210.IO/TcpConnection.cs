using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

namespace WFS210.IO
{
	/// <summary>
	/// TCP Connection that provides communication
	/// in packets instead of raw bytes. 
	/// </summary>
	public class TcpConnection
	{
		/// <summary>
		/// The default gateway address.
		/// </summary>
		const string DefaultAddress = "169.254.1.1";

		/// <summary>
		/// The default port on which the gateway is listening for connections.
		/// </summary>
		const int DefaultPort = 2000;

		/// <summary>
		/// The welcome text.
		/// </summary>
		const string WelcomeText = "*HELLO*";

		/// <summary>
		/// Underlying client used for TCP communication.
		/// </summary>
		protected TcpClient Client;

		/// <summary>
		/// Writer in charge of writing packet objects from the network stream.
		/// </summary>
		protected MessageWriter Writer;

		/// <summary>
		/// The reader.
		/// </summary>
		protected MessageReader Reader;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.TcpConnection"/> class.
		/// </summary>
		public TcpConnection ()
		{

		}

		/// <summary>
		/// Connects using the default address and port.
		/// </summary>
		public bool Connect()
		{
			return Connect (DefaultAddress);
		}

		/// <summary>
		/// Connect to the specified address using the default port.
		/// </summary>
		/// <param name="address">Address.</param>
		public bool Connect(string address)
		{
			return Connect (address, DefaultPort);
		}

		/// <summary>
		/// Connect to the specified address and port.
		/// </summary>
		/// <param name="address">Address.</param>
		/// <param name="port">Port.</param>
		public bool Connect(string address, int port)
		{
			IPEndPoint remoteEP = new IPEndPoint(
				IPAddress.Parse(address), port);

			return Connect (remoteEP);
		}

		/// <summary>
		/// Connects to a custom remote network endpoint.
		/// </summary>
		/// <param name="remoteEP">Remote network endpoint</param>
		public bool Connect(IPEndPoint remoteEP)
		{
			Client = new TcpClient ();
			Client.Connect (remoteEP);

			if (Connected) {

				ReadWelcomeMessage ();

				Writer = new MessageWriter (Client.GetStream (), new PacketSerializer ());
				Reader = new MessageReader (Client.GetStream (), new PacketSerializer ());
			}

			return Connected;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="WFS210.IO.TcpConnection"/> is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		public bool Connected {
			get { return ((Client != null) && (Client.Connected)); }
		}

		/// <summary>
		/// Close the active connection.
		/// </summary>
		public void Close()
		{
			if (Connected) {

				Client.Close ();
				Client = null;
			}
		}

		/// <summary>
		/// Write the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
		public void Write (Message message)
		{
			Writer.Write (message);
		}

		/// <summary>
		/// Attempts to read the welcome message from the server.
		/// </summary>
		public void ReadWelcomeMessage ()
		{
			var hello = new BinaryReader (Client.GetStream ());

			string message = Encoding.ASCII.GetString (hello.ReadBytes (7));
			if (!message.Equals (TcpConnection.WelcomeText)) {
				throw new InvalidOperationException ();
			}
		}

		/// <summary>
		/// Read a message.
		/// </summary>
		public Message Read ()
		{
			if (Client.Available <= 0) {
				return null;
			}

			return Reader.Read ();
		}
	}
}

