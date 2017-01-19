using System;

namespace WFS210.IO
{
	public struct Packet
	{
		public byte STX;
		public byte Command;
		public UInt16 Size;
		public byte[] Data;
		public byte Checksum;
		public byte ETX;
	}
}

