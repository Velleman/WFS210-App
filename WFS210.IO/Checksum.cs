using System;

namespace WFS210.IO
{
	/// <summary>
	/// Checksum implements a checksum algorithm that is specific
	/// to Velbus. This should become an abstract class or interface.
	/// </summary>
	public class Checksum
	{
		private byte checksum;

		public Checksum ()
		{
			Reset ();
		}

		public void Reset()
		{
			checksum = 0;
		}

		public void Update(byte[] buffer, int offset, int count)
		{
			for (int i = 0; i < count; i++) {

				checksum += buffer[offset + i];
			}
		}

		public void Update(int b)
		{
			checksum += (byte)b;
		}

		public byte GetValue()
		{
			return (byte)(~checksum + 1);
		}
	}
}

