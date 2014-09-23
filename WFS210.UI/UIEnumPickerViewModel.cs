using System;

using MonoTouch.UIKit;

namespace WFS210.UI
{

	public class UIEnumPickerViewModel<T> : UIPickerViewModel
	{
		protected readonly T[] Values;

		public event EventHandler<EnumEventArgs<T>> PickerChanged;

		public UIEnumPickerViewModel ()
		{
			Values = (T[])Enum.GetValues (typeof(T));
		}

		public override int GetComponentCount (UIPickerView v)
		{
			return 1;
		}

		public override int GetRowsInComponent (UIPickerView picker, int component)
		{
			return Values.Length;
		}

		public override string GetTitle (UIPickerView picker, int row, int component)
		{
			return Values[row].ToString();
		}

		public override void Selected (UIPickerView picker, int row, int component)
		{
			if (PickerChanged != null) {
				PickerChanged (this, new EnumEventArgs<T> (Values[row]));
			}
		}

	}
}

