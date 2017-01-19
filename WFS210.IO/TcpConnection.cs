using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace WFS210.IO
{
	/// <summary>
	/// TCP Connection that provides communication
	/// in packets instead of raw bytes. 
	/// </summary>
	public class TcpConnection
	{

		public string IPAddress { get; set; }

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
        /// Amount of data that can be parsed
        /// </summary>
        private int AvailableData;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.TcpConnection"/> class.
		/// </summary>
		public TcpConnection ()
		{
			this.IPAddress = DefaultAddress;
		}

		/// <summary>
		/// Connects using the default address and port.
		/// </summary>
		public bool Connect ()
		{
			return Connect (this.IPAddress);
		}

		/// <summary>
		/// Connect to the specified address using the default port.
		/// </summary>
		/// <param name="address">Address.</param>
		public bool Connect (string address)
		{
			return Connect (address, DefaultPort);
		}

		/// <summary>
		/// Connect to the specified address and port.
		/// </summary>
		/// <param name="address">Address.</param>
		/// <param name="port">Port.</param>
		public bool Connect (string address, int port)
		{
			try {
				Client = new TcpClient ();
				var result = Client.BeginConnect (address, port, null, null);
				result.AsyncWaitHandle.WaitOne (TimeSpan.FromSeconds (2));
				if (!Client.Connected) {
					return false;
				}

				Client.EndConnect (result);
				ReadWelcomeMessage ();

				Writer = new MessageWriter (Client.GetStream (), new PacketSerializer ());
				Reader = new MessageReader (Client.GetStream (), new PacketSerializer ());
				return true;
			} catch (Exception e) {
				Console.WriteLine (e.Message);
				return false;
			}
		}

		/// <summary>
		/// Connects to a custom remote network endpoint.
		/// </summary>
		/// <param name="remoteEP">Remote network endpoint</param>
		public bool Connect (IPEndPoint remoteEP)
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
		public void Close ()
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
			try {
				Writer.Write (message);
			} catch (Exception e) {
				Console.WriteLine (e.Message);
			}
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
        /// Loads the Data
        /// </summary>
        public void LoadData()
        {
            AvailableData = Reader.LoadData();
        }

		/// <summary>
		/// Read a message.
		/// </summary>
		public List<Message> ReadMessages ()
		{
            if (AvailableData > 6)
            {
                List<Message> messages = new List<Message>();
                var message = Reader.Read();
                while (message != null)
                {
                    messages.Add(message);
                    message = Reader.Read();
                }
                return messages;
            }
            else
                return null;
		}
	}
}

