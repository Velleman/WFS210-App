using System;

namespace WFS210.IO
{
	public class PacketFactory
	{
		public static Packet CreatePacket(byte command, byte[] data)
		{
			return new Packet (); // TODO: only purpose would be packet framing...
		}
	}
}

