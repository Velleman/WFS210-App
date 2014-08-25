using System;

using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Provides efficient parsing of incoming oscilloscope packets.
	/// </summary>
	public class PacketReader
	{
		private BinaryReader reader;

		private Checksum checksum;

		public Checksum Checksum {
			get { return checksum; }
		}

		public PacketReader (Stream stream, Checksum checksum)
		{
			this.checksum = checksum;
			this.reader = new BinaryReader (new CheckedStream(stream, checksum));
		}

		public Packet ReadPacket()
		{
			// TODO: this is just some placeholder code

			var packet = new Packet ();
			packet.STX = reader.ReadByte ();
			packet.Command = reader.ReadByte ();
			packet.DataLength = reader.ReadUInt16 ();
			packet.Reserved1 = reader.ReadByte ();
			packet.Reserved2 = reader.ReadByte ();
			packet.Data = reader.ReadBytes (packet.DataLength);

			// byte checksum = Checksum.GetValue ();

			packet.Checksum = reader.ReadByte (); // TODO: verify
			packet.ETX = reader.ReadByte ();

			return packet;
		}
	}
}

