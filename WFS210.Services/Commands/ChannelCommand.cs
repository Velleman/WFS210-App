using System;

namespace WFS210.Services
{
	public abstract class ChannelCommand : ServiceCommand
	{
		protected readonly int Channel;

		protected ChannelCommand (int channel)
		{
			this.Channel = channel;
		}
	}
}

