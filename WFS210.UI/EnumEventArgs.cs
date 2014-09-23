using System;

namespace WFS210.UI
{
	public class EnumEventArgs<T> : EventArgs
	{
		public T Value;

		public EnumEventArgs (T value) : base ()
		{
			this.Value = value;
		}
	}
}

