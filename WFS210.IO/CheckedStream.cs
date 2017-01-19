using System;
using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Stream decorator that adds automatic checksum calculation 
	/// of data that has been read or written.
	/// </summary>
	public class CheckedStream : Stream
	{
		/// <summary>
		/// The underlying decorated stream.
		/// </summary>
		private Stream stream;

		/// <summary>
		/// The checksum calculation implementation.
		/// </summary>
		private Checksum checksum;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.CheckedStream"/> class.
		/// </summary>
		/// <param name="stream">Underlying stream object.</param>
		/// <param name="checksum">Checksum implementation.</param>
		public CheckedStream(Stream stream, Checksum checksum)
		{
			this.stream = stream;
			this.checksum = checksum;
		}

		/// <summary>
		/// Checksum implementation.
		/// </summary>
		/// <value>The checksum implementation.</value>
		public Checksum Checksum {
			get { return checksum; }
		}

		public override void Flush()
		{
			stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			// seeking does not influence the checksum
			return stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int len = stream.Read(buffer, offset, count);
			if (len != -1)
				checksum.Update(buffer, offset, len);
			return len;
		}

		public override int ReadByte()
		{
			int b = stream.ReadByte();
			if (b != -1)
				checksum.Update((byte)b);
			return b;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
			checksum.Update(buffer, offset, count);
		}

		public override bool CanRead {
			get { return stream.CanRead; }
		}

		public override bool CanSeek {
			get { return stream.CanRead; }
		}

		public override bool CanWrite {
			get { return stream.CanWrite; }
		}

		public override long Length {
			get { return stream.Length; }
		}

		public override long Position {
			get { return stream.Position; }
			set {
				Seek(value, SeekOrigin.Begin);
			}
		}
	}
}

