using System;

namespace WFS210.UI
{
	public class DismissEventArgs : EventArgs
	{
		public bool RequestCalibrate;

		public DismissEventArgs (bool calibrate)
		{
			this.RequestCalibrate = calibrate;
		}
	}
}

