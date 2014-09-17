using System;

namespace WFS210.Services
{
	public class TriggerChannelCommand : ServiceCommand
	{
		protected readonly int Channel;

		public TriggerChannelCommand (int channel)
		{
			this.Channel = channel;
		}

		public override void Execute(Service service)
		{
			service.Oscilloscope.Trigger.Channel = Channel;
		}
	}
}

