using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Message writer.
	/// </summary>
	public class MessageWriter
	{
		/// <summary>
		/// Stream to which messages are written.
		/// </summary>
		protected readonly Stream Stream;

		/// <summary>
		/// Object in charge of serializing each message.
		/// </summary>
		protected readonly IMessageSerializer Serializer;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.IO.MessageWriter"/> class.
		/// </summary>
		/// <param name="stream">Stream to write to.</param>
		/// <param name="serializer">Serializer in charge of serializing each message.</param>
		public MessageWriter(Stream stream, IMessageSerializer serializer)
		{
			this.Stream = stream;
			this.Serializer = serializer;
		}

		/// <summary>
		/// Writes a message to the underlying stream, using the attached serializer.
		/// </summary>
		/// <param name="message">Message.</param>
		public void Write (Message message)
		{
			Serializer.Serialize (Stream, message);
		}
	}
}

