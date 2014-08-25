using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using WFS210;
namespace WFS210.UI
{
	partial class iWFS210ViewController : UIViewController
	{
		Oscilloscope wfs210;

		public iWFS210ViewController (IntPtr handle) : base (handle)
		{
			wfs210 = new Oscilloscope ();
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ScopeView.setOscilloscope (wfs210);
			wfs210.Channels [0].GenerateTestSignal ();
			ScopeView.UpdateScopeView ();
			ScopeView.BackgroundColor = UIColor.FromRGB (29,29,29);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}


		partial void UIButton6_TouchUpInside (UIButton sender)
		{
			service.Update();
		}
		#endregion
	}
}
