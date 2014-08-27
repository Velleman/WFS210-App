using System;
using System.IO;
using System.Runtime.Serialization;

namespace WFS210.IO
{
	public class MessageWriter
	{
		private Stream stream;

		private MessageSerializer serializer;

		public MessageWriter(Stream stream, MessageSerializer serializer)
		{
			this.stream = stream;
			this.serializer = serializer;
		}

		public void Write (Message message)
		{
			serializer.Serialize (stream, message);
		}
	}
}

