using System;

using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Provides efficient parsing of incoming oscilloscope packets.
	/// </summary>
	public class PacketReader
	{
		protected BinaryReader reader;

		public PacketReader (Stream stream)
		{
			reader = new BinaryReader (stream);
		}

		public Packet ReadPacket()
		{
			var packet = new Packet ();
			packet.STX = reader.ReadByte ();
			packet.Command = reader.ReadByte ();
			packet.DataLength = reader.ReadUInt16 ();
			packet.Reserved1 = reader.ReadByte ();
			packet.Reserved2 = reader.ReadByte ();
			packet.Data = reader.ReadBytes (packet.DataLength);
			packet.Checksum = reader.ReadByte ();
			packet.ETX = reader.ReadByte ();

			return packet;
		}
	}
}

