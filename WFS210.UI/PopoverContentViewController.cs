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
		/// <summary>
		/// The model.
		/// </summary>
		private UIEnumPickerViewModel<T> Model;

		/// <summary>
		/// The picker.
		/// </summary>
		private UIPickerView Picker;

		/// <summary>
		/// Gets the content bounds.
		/// </summary>
		/// <value>The content bounds.</value>
		public RectangleF ContentBounds {
			get { return Picker.Bounds; }
		}

		/// <summary>
		/// Occurs when the value changed.
		/// </summary>
		public event EventHandler<EnumEventArgs<T>> ValueChanged {
			add { Model.PickerChanged += value; }
			remove { Model.PickerChanged -= value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.PopoverContentViewController`1"/> class.
		/// </summary>
		public PopoverContentViewController()
		{
			Model = new UIEnumPickerViewModel<T> ();
			Picker = new UIPickerView ();
			Picker.Model = Model;
			Picker.ShowSelectionIndicator = true;
			Picker.BackgroundColor = new UIColor (62f/255f,60f/255f,60f/255f,255f/255f);
		}

		/// <summary>
		/// the View did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			View.AddSubview (Picker);
		}
	}
}
