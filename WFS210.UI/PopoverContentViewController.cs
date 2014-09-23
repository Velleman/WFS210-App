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

		public RectangleF ContentBounds {
			get { return new RectangleF (0, 0, 200, 350); }
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

			var pMeasurements = new UIPickerView ();
			pMeasurements.Bounds = ContentBounds;
			pMeasurements.Model = Model;

			View.AddSubview (pMeasurements);
		}
	}
}
