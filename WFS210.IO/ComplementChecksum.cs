using System;

namespace WFS210.IO
{
	/// <summary>
	/// Two's complement checksum implementation.
	/// </summary>
	public class ComplementChecksum : Checksum
	{
		/// <summary>
		/// Intermediate checksum value.
		/// </summary>
		private byte checksum;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.ComplementChecksum"/> class.
		/// </summary>
		public ComplementChecksum ()
		{
			Reset ();
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset()
		{
			checksum = 0;
		}

		/// <summary>
		/// Update the checksum with data from the buffer.
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <param name="offset">Offset to start from.</param>
		/// <param name="count">Count bytes to include.</param>
		public void Update(byte[] buffer, int offset, int count)
		{
			for (int i = 0; i < count; i++) {

				checksum += buffer[offset + i];
			}
		}

		/// <summary>
		/// Update the checksum with a single value.
		/// </summary>
		/// <param name="value">The value.</param>
		public void Update(byte value)
		{
			checksum += value;
		}

		/// <summary>
		/// Gets the calculated checksum.
		/// </summary>
		/// <returns>The checksum.</returns>
		public byte GetValue()
		{
			return (byte)(~checksum + 1);
		}
	}
}

