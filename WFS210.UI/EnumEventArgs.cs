using System;

namespace WFS210.UI
{
	public class EnumEventArgs<T>
	{
		public T Value;

		public EnumEventArgs (T value)
		{
			this.Value = value;
		}
	}
}

