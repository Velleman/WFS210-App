using System;

using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Provides common functionality to write packets to a stream.
	/// </summary>
	public class PacketWriter
	{
		protected BinaryWriter writer;

		public PacketWriter (Stream stream)
		{
			writer = new BinaryWriter (stream);
		}

		public void WritePacket(Packet packet)
		{
			Protocol.Frame (packet);

			writer.Write (packet.STX);
			writer.Write (packet.Command);
			writer.Write (packet.DataLength);
			writer.Write (packet.Reserved1);
			writer.Write (packet.Reserved2);
			writer.Write (packet.Data);
			writer.Write (packet.Checksum);
			writer.Write (packet.ETX);
		}
	}
}

