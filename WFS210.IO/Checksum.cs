using System;

namespace WFS210.IO
{
	/// <summary>
	/// Checksum provides an interface for any number of checksum
	/// algorithms specific to your application.
	/// </summary>
	public interface Checksum
	{
		/// <summary>
		/// Resets the checksum algorithm.
		/// </summary>
		void Reset();

		/// <summary>
		/// Update the checksum using the specified buffer, offset and count.
		/// </summary>
		/// <param name="buffer">Data to include in the checksum.</param>
		/// <param name="offset">Offset to start from.</param>
		/// <param name="count">Count bytes to include.</param>
		void Update(byte[] buffer, int offset, int count);

		/// <summary>
		/// Update the checksum using a single byte value.
		/// </summary>
		/// <param name="value">Value to include in the checksum.</param>
		void Update(byte value);

		/// <summary>
		/// Gets the calculated checksum value.
		/// </summary>
		/// <returns>The checksum.</returns>
		byte GetValue();
	}
}

