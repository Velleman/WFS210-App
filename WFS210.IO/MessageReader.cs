using System.IO;

namespace WFS210.IO
{
	public class MessageReader
	{
		protected readonly Stream Stream;

		protected readonly MessageSerializer Serializer;

		public MessageReader(Stream stream, MessageSerializer serializer)
		{
			this.Stream = stream;
			this.Serializer = serializer;
		}

		public Message Read ()
		{
			return Serializer.Deserialize (Stream);
		}
	}
}

