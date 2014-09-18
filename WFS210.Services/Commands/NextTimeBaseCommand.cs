﻿using System;

using WFS210.Util;

namespace WFS210.Services
{
	public class NextTimeBaseCommand : ServiceCommand
	{
		public override void Execute(Service service)
		{
			service.Oscilloscope.TimeBase = service.Oscilloscope.TimeBase.Cycle (1);
		}
	}
}
