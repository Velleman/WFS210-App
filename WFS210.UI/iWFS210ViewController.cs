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
			MainView.BackgroundColor = UIColor.FromPatternImage (UIImage.FromFile ("BACKGROUND/BG-0x0.png"));
			//ScopeView.setOscilloscope (wfs210);
			//wfs210.Channels [0].GenerateTestSignal ();
			//ScopeView.UpdateScopeView ();
			//ScopeView.BackgroundColor = UIColor.FromRGB (29,29,29);
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
		partial void btnSelectChannel2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnSelectChannel1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnAC1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnDC1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnGND1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnProbe1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnVoltDown1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnVoltUp1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTriggerCH1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTriggerCH2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTriggerSlopeUp_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTriggerSlopeDown_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}


		partial void btnTriggerRun_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTriggerNrml_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTriggerOnce_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTriggerHold_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTimeLeft_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnTimeRight_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnAutorange_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnAC2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnDC2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnGND2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnProbe2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnVoltDown2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnVoltUp2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnSettings_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}
