using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace WFS210.UI
{
	public class PopoverContentViewController<T> : UIViewController
	{
		private UIEnumPickerViewModel<T> Model;

		private UIPickerView Picker;

		public RectangleF ContentBounds {
			get { return Picker.Bounds; }
		}

		public event EventHandler<EnumEventArgs<T>> ValueChanged {
			add { Model.PickerChanged += value; }
			remove { Model.PickerChanged -= value; }
		}

		public PopoverContentViewController()
		{
			Model = new UIEnumPickerViewModel<T> ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Picker = new UIPickerView ();
			Picker.Model = Model;
			View.AddSubview (Picker);
		}
	}
}
