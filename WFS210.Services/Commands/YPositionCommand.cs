using System;

namespace WFS210.Services
{
	public class YPositionCommand : ChannelCommand
	{
		protected readonly int YPosition;

		public YPositionCommand (int channel,int yPos) : base (channel)
		{
			this.YPosition = yPos;
		}

		#region implemented abstract members of ServiceCommand

		public override void Execute (Service service)
		{
			service.Oscilloscope.Channels [Channel].YPosition = YPosition;
		}

		#endregion
	}
}

