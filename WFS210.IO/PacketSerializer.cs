using System;
using System.IO;

namespace WFS210.IO
{
	public class PacketSerializer : MessageSerializer
	{
		public void Serialize (System.IO.Stream stream, Message message)
		{
			CheckedStream checkedStream = new CheckedStream (stream, new ComplementChecksum());
			BinaryWriter writer = new BinaryWriter (checkedStream);

			Packet packet;
			packet.STX = 0x02;
			packet.Command = message.Command;
			packet.DataLength = (UInt16)message.Payload.Length;
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
		}

		public Message Deserialize (System.IO.Stream stream)
		{
			throw new NotImplementedException ();
		}
	}
}

