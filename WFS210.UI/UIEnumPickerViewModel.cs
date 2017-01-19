using System;

using UIKit;
using CoreGraphics;

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

		public override nint GetComponentCount (UIPickerView v)
		{
			return 1;
		}

		public override UIView GetView (UIPickerView picker, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel(new CGRect(0, 0, picker.Bounds.Width, 40f));
			lbl.TextColor = UIColor.White;
			lbl.Font = UIFont.SystemFontOfSize(20f);
			lbl.TextAlignment = UITextAlignment.Center;
			lbl.BackgroundColor = new UIColor (62f/255f,60f/255f,60f/255f,128f/255f);
			lbl.Text = Values[row].ToString();
			return lbl;
		}

		public override nint GetRowsInComponent (UIPickerView picker, nint component)
		{
			return Values.Length;
		}

		public override void Selected (UIPickerView picker, nint row, nint component)
		{
			if (PickerChanged != null) {
				PickerChanged (this, new EnumEventArgs<T> (Values[row]));
			}
		}

	}
}

