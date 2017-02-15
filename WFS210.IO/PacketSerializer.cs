using System;
using System.Collections.Generic;
using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Packet serializer that knows how to both serialize and deserialize
	/// messages from a stream.
	/// </summary>
	public class PacketSerializer : MessageSerializer
	{
		private List<Byte> packetBuffer = new List<byte>();
		/// <summary>
		/// Serialize the specified message and send it over the stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="message">Message.</param>
		public override void Serialize(Stream stream, Message message)
		{
			var buffer = new BufferedStream(stream);
			var checkedStream = new CheckedStream(buffer, new ComplementChecksum());

			var writer = new BinaryWriter(checkedStream);

			Packet packet;
			packet.STX = 0x02;
			packet.Command = message.Command;
			packet.Size = (UInt16)(6 + (message.Payload == null ? 0 : message.Payload.Length));
			packet.Data = message.Payload;

			writer.Write(packet.STX);
			writer.Write(packet.Command);
			writer.Write(packet.Size);

			if (packet.Data != null)
			{
				writer.Write(packet.Data);
			}

			packet.Checksum = checkedStream.Checksum.GetValue();
			packet.ETX = 0x0a;

			writer.Write(packet.Checksum);
			writer.Write(packet.ETX);

			buffer.Flush(); // writes the entire packet in one go
		}

		/// <summary>
		/// Deserialize a message from the specified stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		public override Message Deserialize()
		{

			//Seek for the first stx
			while (packetBuffer[0] != 0x02)
			{
				packetBuffer.RemoveAt(0);
				if (packetBuffer.Count == 0)
					return null;
			}

			//Stx found check if valid packet could be made
			if (packetBuffer.Count > 6)
			{
				var size = ReadSize(packetBuffer.ToArray());
				if (size > 6 && size < 1500)
				{
					if (size < packetBuffer.Count)
					{
						//Check if there is an ETX
						if (packetBuffer[size - 1] == 0x0a)
						{
							var checksum = calculateCheckSum(packetBuffer.GetRange(0, size).ToArray());
							if (checksum == packetBuffer[size - 2])
							{
								var packet = GeneratePacket(packetBuffer.ToArray());
								packetBuffer.RemoveRange(0, size);
								return new Message(packet.Command, packet.Data);
							}
							else
							{
								packetBuffer.RemoveAt(0);
								return null;
							}
						}
						else
						{
							packetBuffer.RemoveAt(0);
							return null;
						}
					}
					else
					{
						return null;
					}
				}
				else
				{
					packetBuffer.RemoveAt(0);
					return null;
				}

			}
			else
			{
				return null;
			}
		}

		public override int LoadData(Stream stream)
		{
			var data = new byte[5120];
			var actualsize = stream.Read(data, 0, data.Length);
            if (actualsize != 0)
            {
                packetBuffer.AddRange(data);
            }
			return actualsize;
		}

		private Packet GeneratePacket(byte[] packetBuffer)
		{
			Packet packet = new Packet();
			packet.STX = packetBuffer[0];
			packet.Command = packetBuffer[1];
			packet.Size = (ushort)ReadSize(packetBuffer);
			byte[] data = new byte[packet.Size - 6];
			for (int i = 4; i < packet.Size - 2; i++)
			{
				data[i - 4] = packetBuffer[i];
			}
			packet.Data = data;
			packet.Checksum = calculateCheckSum(packetBuffer);
			packet.ETX = packetBuffer[packet.Size - 1];
			return packet;
		}

		/// <summary>
		/// Reads the size of the given Byte List
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns>The size </returns>
		private int ReadSize(byte[] buffer)
		{
			var sizeLow = buffer[2];
			var sizeHigh = buffer[3];
			var size = (sizeHigh * 256) + sizeLow;
			return size;
		}

		/// <summary>
		/// Calculates the checksum of the given byte array
		/// </summary>
		/// <param name="data"></param>
		/// <returns>The Checksum</returns>
		public byte calculateCheckSum(byte[] data)
		{
			int chkSum = 0;
			int length = data.Length;
			for (int i = 0; i < length - 2; i++)
			{
				chkSum = (chkSum + data[i]);
			}
			chkSum = ~chkSum;
			chkSum += 1;
			return (byte)chkSum;
		}
	}
}

