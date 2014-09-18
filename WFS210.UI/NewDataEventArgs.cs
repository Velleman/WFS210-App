using System;

namespace WFS210.UI
{
	public class NewDataEventArgs : EventArgs
	{

		public int zeroLine1;
		public int zeroLine2;
		public int triggerLevel;
		public NewDataEventArgs (int zero1,int zero2, int trigger)
		{
			zeroLine1 = zero1;
			zeroLine2 = zero2;
			triggerLevel = trigger;
		}
	}
}

