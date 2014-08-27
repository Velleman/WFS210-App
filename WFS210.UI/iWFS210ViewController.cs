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

		#endregion

		#region Events Channel1

		partial void btnSelectChannel1_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnAC1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.AC;
		}

		partial void btnDC1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.DC;
		}

		partial void btnGND1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.GND;
		}

		partial void btnProbe1_TouchUpInside (UIButton sender)
		{
			if(wfs210.Channels[0].AttenuationFactor == AttenuationFactor.X10)
				wfs210.Channels[0].AttenuationFactor = AttenuationFactor.X1;
			else
				wfs210.Channels[0].AttenuationFactor = AttenuationFactor.X10;
		}

		partial void btnVoltDown1_TouchUpInside (UIButton sender)
		{
			if(wfs210.Channels[0].VoltsPerDivision != VoltsPerDivision.VdivNone)
				wfs210.Channels[0].VoltsPerDivision = wfs210.Channels[0].VoltsPerDivision +1;
		}

		partial void btnVoltUp1_TouchUpInside (UIButton sender)
		{
			if(wfs210.Channels[0].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
			wfs210.Channels[0].VoltsPerDivision = wfs210.Channels[0].VoltsPerDivision +1;
		}

		#endregion

		#region Events Trigger

		partial void btnTriggerCH1_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Channel = 1;
		}

		partial void btnTriggerCH2_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Channel = 2;
		}

		partial void btnTriggerSlopeUp_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Slope = TriggerSlope.Rising;
		}

		partial void btnTriggerSlopeDown_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Slope = TriggerSlope.Falling;
		}

		partial void btnTriggerRun_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Run;
		}

		partial void btnTriggerNrml_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Normal;
		}

		partial void btnTriggerOnce_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Once;
		}

		partial void btnTriggerHold_TouchUpInside (UIButton sender)
		{
			if(wfs210.Hold)
				wfs210.Hold = false;
			else
				wfs210.Hold = true;
		}

		partial void btnTimeLeft_TouchUpInside (UIButton sender)
		{
			wfs210.TimeBase = wfs210.TimeBase - 1;
		}

		partial void btnTimeRight_TouchUpInside (UIButton sender)
		{
			wfs210.TimeBase = wfs210.TimeBase + 1;
		}

		partial void btnAutorange_TouchUpInside (UIButton sender)
		{
			if (wfs210.AutoRange)
				wfs210.AutoRange = false;
			else
				wfs210.AutoRange = true;
		}

		#endregion

		#region Events Channel2

		partial void btnSelectChannel2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException();
		}

		partial void btnAC2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.AC;
		}

		partial void btnDC2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.DC;
		}

		partial void btnGND2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.GND;
		}

		partial void btnProbe2_TouchUpInside (UIButton sender)
		{
			if(wfs210.Channels[1].AttenuationFactor == AttenuationFactor.X10)
				wfs210.Channels[1].AttenuationFactor = AttenuationFactor.X1;
			else
				wfs210.Channels[1].AttenuationFactor = AttenuationFactor.X10;
		}

		partial void btnVoltDown2_TouchUpInside (UIButton sender)
		{
			if(wfs210.Channels[1].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
				wfs210.Channels[1].VoltsPerDivision = wfs210.Channels[1].VoltsPerDivision +1;
		}

		partial void btnVoltUp2_TouchUpInside (UIButton sender)
		{
			if(wfs210.Channels[1].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
				wfs210.Channels[1].VoltsPerDivision = wfs210.Channels[1].VoltsPerDivision +1;
		}

		#endregion

		#region Events Settings

		partial void btnSettings_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();//TODO segue to settings
		}

		#endregion
	}
}
