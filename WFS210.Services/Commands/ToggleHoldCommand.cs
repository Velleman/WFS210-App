namespace WFS210.Services
{
	public class ToggleHoldCommand : ServiceCommand
	{
		public override void Execute(Service service)
		{
			service.Oscilloscope.Hold = !service.Oscilloscope.Hold;
		}
	}
}

