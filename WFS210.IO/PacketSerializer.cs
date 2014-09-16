using System;
using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Packet serializer that knows how to both serialize and deserialize
	/// messages from a stream.
	/// </summary>
	public class PacketSerializer : IMessageSerializer
	{
		/// <summary>
		/// Serialize the specified message and send it over the stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="message">Message.</param>
		public void Serialize (Stream stream, Message message)
		{
			var buffer = new BufferedStream (stream);
			var checkedStream = new CheckedStream (buffer, new ComplementChecksum());

			var writer = new BinaryWriter (checkedStream);

			Packet packet;
			packet.STX = 0x02;
			packet.Command = message.Command;
			packet.Size = (UInt16)(8 + (message.Payload == null ? 0 : message.Payload.Length));
			packet.Reserved1 = 0x00;
			packet.Reserved2 = 0x00;
			packet.Data = message.Payload;

			writer.Write (packet.STX);
			writer.Write (packet.Command);
			writer.Write (packet.Size);
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

		/// <summary>
		/// Deserialize a message from the specified stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		public Message Deserialize (Stream stream)
		{
			var checkedStream = new CheckedStream (stream, new ComplementChecksum ());
			var reader = new BinaryReader (checkedStream);

			Packet packet;
			packet.STX = reader.ReadByte ();
			packet.Command = reader.ReadByte ();
			packet.Size = reader.ReadUInt16 ();
			packet.Reserved1 = reader.ReadByte ();
			packet.Reserved2 = reader.ReadByte ();
			packet.Data = reader.ReadBytes (packet.Size - 8);

			byte expectedChecksum = checkedStream.Checksum.GetValue ();

			packet.Checksum = reader.ReadByte ();
			if (packet.Checksum != expectedChecksum) {

			}

			packet.ETX = reader.ReadByte ();

			return new Message (packet.Command, packet.Data);
		}
	}
}

