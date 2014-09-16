using System;
using System.IO;

namespace WFS210.IO
{
	/// <summary>
	/// Message object serializer.
	/// </summary>
	public interface IMessageSerializer
	{
		/// <summary>
		/// Serialize the specified message and send it over the stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="message">Message.</param>
		void Serialize(Stream stream, Message message);

		/// <summary>
		/// Deserialize a message from the specified stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		Message Deserialize(Stream stream);
	}
}

