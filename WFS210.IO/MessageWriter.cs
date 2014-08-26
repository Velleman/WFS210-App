using System;
using System.IO;
using System.Runtime.Serialization;

namespace WFS210.IO
{
	public class MessageWriter
	{
		private Stream stream;

		private MessageFormatter formatter;

		public MessageWriter(Stream stream, MessageFormatter formatter)
		{
			this.stream = stream;
			this.formatter = formatter;
		}

		public void Write (Message message)
		{
			formatter.Serialize (stream, message);
		}
	}
}

