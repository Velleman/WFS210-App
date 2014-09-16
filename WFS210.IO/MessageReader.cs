using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Message reader.
	/// </summary>
	public class MessageReader
	{
		/// <summary>
		/// Stream from which messages are read.
		/// </summary>
		protected readonly Stream Stream;

		/// <summary>
		/// Object in charge of deserializing messages from the stream.
		/// </summary>
		protected readonly MessageSerializer Serializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.MessageReader"/> class.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="serializer">Serializer.</param>
		public MessageReader(Stream stream, MessageSerializer serializer)
		{
			this.Stream = stream;
			this.Serializer = serializer;
		}

		/// <summary>
		/// Reads a single message from the underlying stream.
		/// </summary>
		public Message Read ()
		{
			return Serializer.Deserialize (Stream);
		}
	}
}

