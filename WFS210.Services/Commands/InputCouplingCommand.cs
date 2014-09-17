using System;

namespace WFS210.Services
{
	public class InputCouplingCommand : ChannelCommand
	{
		protected readonly InputCoupling InputCoupling;

		public InputCouplingCommand (int channel, InputCoupling inputCoupling) : base(channel)
		{
			this.InputCoupling = inputCoupling;
		}

		public override void Execute(Service service)
		{
			service.Oscilloscope.Channels[Channel].InputCoupling = InputCoupling;
		}
	}
}

