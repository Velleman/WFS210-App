using WFS210.Util;

namespace WFS210.Services
{
	public class PreviousVoltsPerDivisionCommand : ChannelCommand
	{
		public PreviousVoltsPerDivisionCommand (int channel) : base(channel)
		{

		}

		public override void Execute(Service service)
		{
			if (service.Oscilloscope.AutoRange)
				service.Oscilloscope.AutoRange = !service.Oscilloscope.AutoRange;
			Channel channel = service.Oscilloscope.Channels [Channel];

			channel.VoltsPerDivision = channel.VoltsPerDivision.Cycle (-1);
		}
	}
}

