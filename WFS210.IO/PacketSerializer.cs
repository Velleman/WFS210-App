using System;
using System.IO;

namespace WFS210.IO
{
	public class PacketSerializer : MessageSerializer
	{
		public void Serialize (Stream stream, Message message)
		{
			BufferedStream buffer = new BufferedStream (stream);
			CheckedStream checkedStream = new CheckedStream (buffer, new ComplementChecksum());

			BinaryWriter writer = new BinaryWriter (checkedStream);

			Packet packet;
			packet.STX = 0x02;
			packet.Command = message.Command;
			packet.DataLength = (UInt16)(8 + (message.Payload == null ? 0 : message.Payload.Length));
			packet.Reserved1 = 0x00;
			packet.Reserved2 = 0x00;
			packet.Data = message.Payload;

			writer.Write (packet.STX);
			writer.Write (packet.Command);
			writer.Write (packet.DataLength);
			writer.Write (packet.Reserved1);
			writer.Write (packet.Reserved2);

			if (packet.Data != null) {
				writer.Write (packet.Data);
			}

			packet.Checksum = checkedStream.Checksum.GetValue ();
			packet.ETX = 0x0a;

			writer.Write (packet.Checksum);
			writer.Write (packet.ETX);

			buffer.Flush (); // writes the entire packet in one go
		}

		public Message Deserialize (Stream stream)
		{
			CheckedStream checkedStream = new CheckedStream (stream, new ComplementChecksum ());
			BinaryReader reader = new BinaryReader (checkedStream);

			Packet packet;
			packet.STX = reader.ReadByte ();
			packet.Command = reader.ReadByte ();
			packet.DataLength = reader.ReadUInt16 ();
			packet.Reserved1 = reader.ReadByte ();
			packet.Reserved2 = reader.ReadByte ();
			packet.Data = reader.ReadBytes (packet.DataLength);

			byte expectedChecksum = checkedStream.Checksum.GetValue ();

			packet.Checksum = reader.ReadByte ();
			if (packet.Checksum != expectedChecksum) {

			}

			packet.ETX = reader.ReadByte ();

			return new Message (packet.Command, packet.Data);
		}
	}
}

