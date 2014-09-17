using System;

namespace WFS210.Services
{
	public class TriggerModeCommand : ServiceCommand
	{
		protected readonly TriggerMode TriggerMode;

		public TriggerModeCommand (TriggerMode triggerMode)
		{
			this.TriggerMode = triggerMode;
		}

		public override void Execute(Service service)
		{
			service.Oscilloscope.Trigger.Mode = TriggerMode;
		}
	}
}

