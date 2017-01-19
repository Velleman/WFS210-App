using WFS210;
using WFS210.Services;

namespace WFS210.Services
{
	public class TriggerSlopeCommand : ServiceCommand
	{
		protected readonly TriggerSlope TriggerSlope;

		public TriggerSlopeCommand (TriggerSlope triggerSlope)
		{
			this.TriggerSlope = triggerSlope;
		}

		public override void Execute(Service service)
		{
			service.Oscilloscope.Trigger.Slope = TriggerSlope;
		}
	}
}

