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
		public abstract void Serialize(Stream stream, Message message);

		/// <summary>
		/// Deserialize a message from the specified stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		public abstract Message Deserialize();

        /// <summary>
        /// Loads a chunk of data of the stream
        /// </summary>
        /// <param name="stream"></param>
        public abstract int LoadData(Stream stream);
	}
}

