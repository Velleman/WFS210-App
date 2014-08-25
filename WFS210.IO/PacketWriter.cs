using System;

using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Provides common functionality to write packets to a stream.
	/// </summary>
	public class PacketWriter
	{
		private BinaryWriter writer;

		private Checksum checksum;

		public Checksum Checksum {
			get { return checksum; }
		}

		public PacketWriter (Stream stream, Checksum checksum)
		{
			this.checksum = checksum;
			this.writer = new BinaryWriter (new CheckedStream(stream, checksum));
		}

		public void WritePacket(Packet packet)
		{
			Checksum.Reset ();

			writer.Write (packet.STX);
			writer.Write (packet.Command);
			writer.Write (packet.DataLength);
			writer.Write (packet.Reserved1);
			writer.Write (packet.Reserved2);
			writer.Write (packet.Data);

			// Calculate the checksum with what has been
			// written so far to the stream.
			packet.Checksum = Checksum.GetValue ();
			
			writer.Write (packet.Checksum);
			writer.Write (packet.ETX);
		}
	}
}

