using WFS210;

namespace WFS210.Services
{
	public class FlipAttenuationFactorCommand : ChannelCommand
	{
		public FlipAttenuationFactorCommand (int channel) : base(channel)
		{

		}

		public override void Execute(Service service)
		{
			Channel channel = service.Oscilloscope.Channels [Channel];

			channel.AttenuationFactor =
				((channel.AttenuationFactor == AttenuationFactor.X1)
						? AttenuationFactor.X10
						: AttenuationFactor.X1);
		}
	}
}

