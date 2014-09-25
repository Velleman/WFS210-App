using System;

namespace WFS210.Services
{
	public class TriggerLevelCommand : ServiceCommand
	{
		protected readonly int TriggerLevel;

		public TriggerLevelCommand (int triggerLevel)
		{
			this.TriggerLevel = triggerLevel;
		}

		public override void Execute (Service service)
		{
			service.Oscilloscope.Trigger.Level = TriggerLevel;
		}
	}
}

