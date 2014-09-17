using System;
using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Message object serializer.
	/// </summary>
	public abstract class MessageSerializer
	{
		/// <summary>
		/// Serialize the specified message and send it over the stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="message">Message.</param>
		abstract public void Serialize(Stream stream, Message message);

		/// <summary>
		/// Deserialize a message from the specified stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		abstract public Message Deserialize(Stream stream);
	}
}

