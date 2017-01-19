using System;

namespace WFS210.UI
{
	public class EnumEventArgs<T> : EventArgs
	{
		/// <summary>
		/// The value.
		/// </summary>
		public T Value;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.EnumEventArgs`1"/> class.
		/// </summary>
		/// <param name="value">Value.</param>
		public EnumEventArgs (T value) : base ()
		{
			this.Value = value;
		}
	}
}

