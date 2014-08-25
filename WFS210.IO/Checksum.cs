using System;

namespace WFS210.IO
{
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

		public void Update(byte[] data, int size)
		{
			foreach (byte value in data) {

				checksum += value;
			}
		}

		public byte GetValue()
		{
			return (byte)(~checksum + 1);
		}
	}
}

