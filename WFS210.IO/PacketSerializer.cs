using System;
using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Packet serializer that knows how to both serialize and deserialize
	/// messages from a stream.
	/// </summary>
	public class PacketSerializer : MessageSerializer
	{
		/// <summary>
		/// Serialize the specified message and send it over the stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="message">Message.</param>
		public override void Serialize (Stream stream, Message message)
		{
			var buffer = new BufferedStream (stream);
			var checkedStream = new CheckedStream (buffer, new ComplementChecksum ());

			var writer = new BinaryWriter (checkedStream);

			Packet packet;
			packet.STX = 0x02;
			packet.Command = message.Command;
			packet.Size = (UInt16)(6 + (message.Payload == null ? 0 : message.Payload.Length));
			packet.Data = message.Payload;

			writer.Write (packet.STX);
			writer.Write (packet.Command);
			writer.Write (packet.Size);

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
		public override Message Deserialize (Stream stream)
		{
			var checkedStream = new CheckedStream (stream, new ComplementChecksum ());
			var reader = new BinaryReader (checkedStream);

			Packet packet;
			packet.STX = reader.ReadByte ();
			packet.Command = reader.ReadByte ();
			packet.Size = reader.ReadUInt16 ();
			packet.Data = reader.ReadBytes (packet.Size - 6);

			byte expectedChecksum = checkedStream.Checksum.GetValue ();

			packet.Checksum = reader.ReadByte ();
			if (packet.Checksum != expectedChecksum) {
				Console.WriteLine ("Invalid Packet");
			} else
				packet.ETX = reader.ReadByte ();

			return new Message (packet.Command, packet.Data);
		}
	}
}

