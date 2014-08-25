using System;

namespace WFS210.IO
{
	public class Protocol
	{
		public static readonly byte STX = 0x02;

		public static readonly byte ETX = 0x0a;

		public static void Frame(Packet packet)
		{
			packet.STX = Protocol.STX;
			packet.Checksum = 0x00; // TODO: calculate
			packet.ETX = Protocol.ETX;
		}
	}
}

