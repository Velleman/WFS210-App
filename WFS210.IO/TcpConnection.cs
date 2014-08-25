using System;
using System.Net;
using System.Net.Sockets;

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
		static readonly string DefaultAddress = "169.254.1.1";

		/// <summary>
		/// The default port on which the gateway is listening for connections.
		/// </summary>
		static readonly int DefaultPort = 2000;

		/// <summary>
		/// Underlying client used for TCP communication.
		/// </summary>
		protected TcpClient client;

		/// <summary>
		/// Checksum implementation used to send and receive packet.
		/// </summary>
		protected Checksum checksum;

		/// <summary>
		/// Reader in charge of reading packet objects from the network stream.
		/// </summary>
		protected PacketReader reader;

		/// <summary>
		/// Writer in charge of writing packet objects from the network stream.
		/// </summary>
		protected PacketWriter writer;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.TcpConnection"/> class.
		/// </summary>
		public TcpConnection ()
		{
			this.client = new TcpClient ();
			this.checksum = new Checksum ();
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
			client.Connect (remoteEP);

			if (Connected) {

				reader = new PacketReader (client.GetStream (), checksum);
				writer = new PacketWriter (client.GetStream (), checksum);
			}

			return Connected;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="WFS210.IO.TcpConnection"/> is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		public bool Connected {
			get { return client.Connected; }
		}

		/// <summary>
		/// Close the active connection.
		/// </summary>
		public void Close()
		{
			if (Connected) {

				client.Close ();
			}
		}

		/// <summary>
		/// Attempt to read a packet from the connection.
		/// </summary>
		public Packet Read()
		{
			return reader.ReadPacket();
		}

		/// <summary>
		/// Write a packet to the connection
		/// </summary>
		/// <param name="packet">Packet.</param>
		public void Write(Packet packet)
		{
			writer.WritePacket (packet);
		}
	}
}

