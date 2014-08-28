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
			UpdateScopeControls ();
		}

		partial void btnDC1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.DC;
			UpdateScopeControls ();
		}

		partial void btnGND1_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [0].InputCoupling = InputCoupling.GND;
			UpdateScopeControls ();
		}

		partial void btnProbe1_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [0].AttenuationFactor == AttenuationFactor.X10)
				wfs210.Channels [0].AttenuationFactor = AttenuationFactor.X1;
			else
				wfs210.Channels [0].AttenuationFactor = AttenuationFactor.X10;
			UpdateScopeControls ();
		}

		partial void btnVoltDown1_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [0].VoltsPerDivision != VoltsPerDivision.VdivNone)
				wfs210.Channels [0].VoltsPerDivision = wfs210.Channels [0].VoltsPerDivision + 1;
		}

		partial void btnVoltUp1_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [0].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
				wfs210.Channels [0].VoltsPerDivision = wfs210.Channels [0].VoltsPerDivision + 1;
		}

		#endregion

		#region Events Trigger

		partial void btnTriggerCH1_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Channel = 0;
			UpdateScopeControls();
		}

		partial void btnTriggerCH2_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Channel = 1;
			UpdateScopeControls();
		}

		partial void btnTriggerSlopeUp_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Slope = TriggerSlope.Rising;
			UpdateScopeControls();
		}

		partial void btnTriggerSlopeDown_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Slope = TriggerSlope.Falling;
			UpdateScopeControls();
		}

		partial void btnTriggerRun_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Run;
			UpdateScopeControls();
		}

		partial void btnTriggerNrml_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Normal;
			UpdateScopeControls();
		}

		partial void btnTriggerOnce_TouchUpInside (UIButton sender)
		{
			wfs210.Trigger.Mode = TriggerMode.Once;
			UpdateScopeControls();
		}

		partial void btnTriggerHold_TouchUpInside (UIButton sender)
		{
			if (wfs210.Hold)
				wfs210.Hold = false;
			else
				wfs210.Hold = true;
			UpdateScopeControls();
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
			UpdateScopeControls();
		}

		#endregion

		#region Events Channel2

		partial void btnSelectChannel2_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();
		}

		partial void btnAC2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.AC;
			UpdateScopeControls ();
		}

		partial void btnDC2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.DC;
			UpdateScopeControls ();
		}

		partial void btnGND2_TouchUpInside (UIButton sender)
		{
			wfs210.Channels [1].InputCoupling = InputCoupling.GND;
			UpdateScopeControls ();
		}

		partial void btnProbe2_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [1].AttenuationFactor == AttenuationFactor.X10)
				wfs210.Channels [1].AttenuationFactor = AttenuationFactor.X1;
			else
				wfs210.Channels [1].AttenuationFactor = AttenuationFactor.X10;
			UpdateScopeControls ();
		}

		partial void btnVoltDown2_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [1].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
				wfs210.Channels [1].VoltsPerDivision = wfs210.Channels [1].VoltsPerDivision + 1;
		}

		partial void btnVoltUp2_TouchUpInside (UIButton sender)
		{
			if (wfs210.Channels [1].VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
				wfs210.Channels [1].VoltsPerDivision = wfs210.Channels [1].VoltsPerDivision + 1;
		}

		#endregion

		#region Events Settings

		partial void btnSettings_TouchUpInside (UIButton sender)
		{
			throw new NotImplementedException ();//TODO segue to settings
		}



		#endregion


		private void UpdateScopeControls ()
		{
			UpdateChannel1UI ();
			UpdateChannel2UI ();
			UpdateTriggerUI ();
		}


		void UpdateChannel1UI ()
		{
			UpdateInputCoupling1 ();
			UpdateAttenuationFactor1 ();
		}

		void UpdateChannel2UI ()
		{
			UpdateInputCoupling2 ();
			UpdateAttenuationFactor2 ();
		}

		void UpdateTriggerUI ()
		{
			UpdateTriggerChannelUI ();
			UpdateTriggerSlopeUI ();
			UpdateTriggerModeUI ();
			UpdateHoldUI ();
			UpdateAutorangeUI ();
		}

		void UpdateInputCoupling1 ()
		{
			switch (wfs210.Channels [0].InputCoupling) {
			case InputCoupling.AC:
				btnAC1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-AC-ON-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6.png"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-DC-ON-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6.png"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-GND-ON-263x6.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor1 ()
		{
			switch (wfs210.Channels [0].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-1X-OFF-344x6.png"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 1/CHAN1-10X-ON-344x6.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateInputCoupling2 ()
		{
			switch (wfs210.Channels [1].InputCoupling) {
			case InputCoupling.AC:
				btnAC2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-AC-ON-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710.png"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-DC-ON-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710.png"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-GND-ON-263x710.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor2 ()
		{
			switch (wfs210.Channels [1].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-1X-OFF-344x710.png"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/CHANNEL 2/CHAN2-10X-ON-344x710.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateTriggerChannelUI ()
		{
			if (wfs210.Trigger.Channel == 0) {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-CHAN1-ON-6x96.png"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-CHAN2-OFF-60x96.png"), UIControlState.Normal);
			} else {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-CHAN1-OFF-6x96.png"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-CHAN2-ON-60x96.png"), UIControlState.Normal);
			}
		}

		void UpdateTriggerSlopeUI ()
		{
			if (wfs210.Trigger.Slope == TriggerSlope.Rising) {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-UP-ON-6x156.png"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-DOWN-OFF-60x156.png"), UIControlState.Normal);
			} else {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-UP-OFF-6x156.png"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-DOWN-ON-60x156.png"), UIControlState.Normal);
			}
		}

		void UpdateTriggerModeUI ()
		{
			switch (wfs210.Trigger.Mode) {
			case TriggerMode.Normal:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-NRML-ON-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216.png"), UIControlState.Normal);
				break;
			case TriggerMode.Once:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-ONCE-ON-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216.png"), UIControlState.Normal);
				break;
			case TriggerMode.Run:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-RUN-ON-6x216.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateHoldUI ()
		{
			if (wfs210.Hold)
				btnTriggerHold.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-HOLD-ON-6x336.png"), UIControlState.Normal);
			else
				btnTriggerHold.SetBackgroundImage (UIImage.FromFile ("BUTTONS/TRIGGER/TRIG-HOLD-OFF-6x336.png"), UIControlState.Normal);
		}

		void UpdateAutorangeUI ()
		{
			if (wfs210.AutoRange)
				btnAutorange.SetBackgroundImage (UIImage.FromFile ("BUTTONS/AUTO RANGE/AUTORANGE-ON-6x541.png"), UIControlState.Normal);
			else
				btnAutorange.SetBackgroundImage (UIImage.FromFile ("BUTTONS/AUTO RANGE/AUTORANGE-OFF-6x541.png"), UIControlState.Normal);
		}
	}
}
