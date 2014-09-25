using System;

namespace WFS210.Services
{
	public class TriggerLevelCommand : ServiceCommand
	{
		protected readonly int TriggerLevel;

		public TriggerLevelCommand (int TriggerLevel)
		{
			this.TriggerLevel = TriggerLevel;
		}

		#region implemented abstract members of ServiceCommand

		public override void Execute (Service service)
		{
			service.Oscilloscope.Trigger.Level = TriggerLevel;
		}

		#endregion
	}
}

