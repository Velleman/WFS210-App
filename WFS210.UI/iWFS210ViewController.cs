using System;
using MonoTouch.UIKit;
using WFS210;
using System.Timers;
using WFS210.Services;
using MonoTouch.Foundation;

namespace WFS210.UI
{
	partial class iWFS210ViewController : UIViewController
	{
		/// <summary>
		/// Wfs210 oscilloscope.
		/// </summary>
		protected readonly Oscilloscope Oscilloscope;

		/// <summary>
		/// The service manager.
		/// </summary>
		protected readonly ServiceManager ServiceManager;

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <value>The service.</value>
		public Service Service {
			get { return ServiceManager.ActiveService; }
		}

		SettingsViewController settingsViewController;

		/// <summary>
		/// The display settings.
		/// </summary>
		public readonly DisplaySettings DisplaySettings;

		UIPopoverController DetailViewPopover;


		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.iWFS210ViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public iWFS210ViewController (IntPtr handle) : base (handle)
		{
			this.Oscilloscope = new Oscilloscope ();
			var demo = NSUserDefaults.StandardUserDefaults.BoolForKey ("demo");
			if (demo)
				this.ServiceManager = new ServiceManager (Oscilloscope, ServiceType.Demo);
			else {
				this.ServiceManager = new ServiceManager (Oscilloscope, ServiceType.Live);
				if (!Service.Activate ()) {
					this.ServiceManager.ServiceType = ServiceType.Demo;
					new UIAlertView ("No Connection", "Demo mode is now running", null, "OK", null).Show ();
				} else {
					Service.RequestSettings ();
					Service.RequestSamples ();
				}
			}
			this.DisplaySettings = new DisplaySettings (MarkerUnit.dt, SignalUnit.Vdc);
		}

		void SettingsChanged (object sender, EventArgs e)
		{
			InvokeOnMainThread (() => {
				UpdateScopeControls ();
				ScopeView.UpdateScopeView ();
			});

		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			MainView.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("BACKGROUND/BG-0x0.png"));
			Service.SettingsChanged += SettingsChanged;
			ScopeView.Service = Service;
			ScopeView.Initialize ();
			var timer = new System.Timers.Timer (500);
			timer.Elapsed += (object sender, ElapsedEventArgs e) => {
				Service.Update ();
				InvokeOnMainThread (ScopeView.UpdateScopeView);
				UpdateMeasurements ();
			};
			timer.Enabled = true;
			timer.Start ();

			ScopeView.SelectedChannel = 0;

			ScopeView.NewData += (object sender, NewDataEventArgs e) => UpdateScopeControls ();

		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			UpdateScopeControls ();
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		#endregion

		#region Events Channel1

		partial void btnSelectChannel1_TouchUpInside (UIButton sender)
		{
			ScopeView.SelectedChannel = 0;
			UpdateScopeControls ();
		}

		partial void btnAC1_TouchUpInside (UIButton sender)
		{
			Service.Execute (new InputCouplingCommand (0, InputCoupling.AC));
		}

		partial void btnDC1_TouchUpInside (UIButton sender)
		{
			Service.Execute (new InputCouplingCommand (0, InputCoupling.DC));
		}

		partial void btnGND1_TouchUpInside (UIButton sender)
		{
			Service.Execute (new InputCouplingCommand (0, InputCoupling.GND));
		}

		partial void btnProbe1_TouchUpInside (UIButton sender)
		{
			Service.Execute (new FlipAttenuationFactorCommand (0));
		}

		partial void btnVoltDown1_TouchUpInside (UIButton sender)
		{
			Service.Execute (new NextVoltsPerDivisionCommand (0));
		}

		partial void btnVoltUp1_TouchUpInside (UIButton sender)
		{
			Service.Execute (new PreviousVoltsPerDivisionCommand (0));
		}

		partial void btnMarkerMeasurements_TouchUpInside (UIButton sender)
		{
			ShowMarkerUnitPopover (sender, 0);
		}

		partial void btnSignalMeasurements_TouchUpInside (UIButton sender)
		{
			ShowSignalUnitPopover (sender, 0);
		}

		#endregion

		#region Events Trigger

		partial void btnTriggerCH1_TouchUpInside (UIButton sender)
		{
			Service.Execute (new TriggerChannelCommand (0));
		}

		partial void btnTriggerCH2_TouchUpInside (UIButton sender)
		{
			Service.Execute (new TriggerChannelCommand (1));
		}

		partial void btnTriggerSlopeUp_TouchUpInside (UIButton sender)
		{
			Service.Execute (new TriggerSlopeCommand (TriggerSlope.Rising));
		}

		partial void btnTriggerSlopeDown_TouchUpInside (UIButton sender)
		{
			Service.Execute (new TriggerSlopeCommand (TriggerSlope.Falling)); 
		}

		partial void btnTriggerRun_TouchUpInside (UIButton sender)
		{
			Service.Execute (new TriggerModeCommand (TriggerMode.Run));
		}

		partial void btnTriggerNrml_TouchUpInside (UIButton sender)
		{
			Service.Execute (new TriggerModeCommand (TriggerMode.Normal));
		}

		partial void btnTriggerOnce_TouchUpInside (UIButton sender)
		{
			Service.Execute (new TriggerModeCommand (TriggerMode.Once));
		}

		partial void btnTriggerHold_TouchUpInside (UIButton sender)
		{
			Service.Execute (new ToggleHoldCommand ());
		}

		partial void btnTimeLeft_TouchUpInside (UIButton sender)
		{
			Service.Execute (new PreviousTimeBaseCommand ());
		}

		partial void btnTimeRight_TouchUpInside (UIButton sender)
		{
			Service.Execute (new NextTimeBaseCommand ());
		}

		partial void btnAutorange_TouchUpInside (UIButton sender)
		{
			Oscilloscope.AutoRange = !Oscilloscope.AutoRange;
			Service.ApplySettings ();  
		}

		#endregion

		#region Events Channel2

		partial void btnSelectChannel2_TouchUpInside (UIButton sender)
		{
			ScopeView.SelectedChannel = 1;
			UpdateScopeControls ();
		}

		partial void btnAC2_TouchUpInside (UIButton sender)
		{
			Service.Execute (new InputCouplingCommand (1, InputCoupling.AC));
		}

		partial void btnDC2_TouchUpInside (UIButton sender)
		{
			Service.Execute (new InputCouplingCommand (1, InputCoupling.DC));
		}

		partial void btnGND2_TouchUpInside (UIButton sender)
		{
			Service.Execute (new InputCouplingCommand (1, InputCoupling.GND));
		}

		partial void btnProbe2_TouchUpInside (UIButton sender)
		{
			Service.Execute (new FlipAttenuationFactorCommand (1));
		}

		partial void btnVoltDown2_TouchUpInside (UIButton sender)
		{
			Service.Execute (new NextVoltsPerDivisionCommand (1));
		}

		partial void btnVoltUp2_TouchUpInside (UIButton sender)
		{
			Service.Execute (new PreviousVoltsPerDivisionCommand (1));
		}


		partial void btnMarkerMeasurements2_TouchUpInside (UIButton sender)
		{
			ShowMarkerUnitPopover (sender, 1);
		}

		partial void btnSignalMeasurements2_TouchUpInside (UIButton sender)
		{
			ShowSignalUnitPopover (sender, 1);
		}

		private void ShowSignalUnitPopover (UIView view, int channel)
		{
			var content = new PopoverContentViewController<SignalUnit> ();
			content.ValueChanged += (object s, EnumEventArgs<SignalUnit> e) => {
				DisplaySettings.SignalUnits[channel] = e.Value;
				UpdateMeasurements ();
			};

			DetailViewPopover = new UIPopoverController (content);
			DetailViewPopover.PopoverContentSize = content.ContentBounds.Size;

			DetailViewPopover.PresentFromRect (view.Frame, View, UIPopoverArrowDirection.Any, true);
		}

		private void ShowMarkerUnitPopover (UIView view, int channel)
		{
			var content = new PopoverContentViewController<MarkerUnit> ();
			content.ValueChanged += (object s, EnumEventArgs<MarkerUnit> e) => {
				DisplaySettings.MarkerUnits[channel] = e.Value;
				UpdateMeasurements ();
			};

			DetailViewPopover = new UIPopoverController (content);
			DetailViewPopover.PopoverContentSize = content.ContentBounds.Size;

			DetailViewPopover.PresentFromRect (view.Frame, View, UIPopoverArrowDirection.Any, true);
		}

		#endregion

		#region Events Settings

		partial void btnSettings_TouchUpInside (UIButton sender)
		{
			settingsViewController = this.Storyboard.InstantiateViewController ("SettingsViewController") as SettingsViewController;
			settingsViewController.WifiSetting = Oscilloscope.WifiSetting;
			settingsViewController.ServiceManager = this.ServiceManager;
			settingsViewController.RequestedDismiss += (object s, EventArgs e) => settingsViewController.DismissViewController(true,null);
			PresentViewController (settingsViewController, true, null);
		}

		partial void btnSnap_TouchUpInside (UIButton sender)
		{
			UIGraphics.BeginImageContext (UIScreen.MainScreen.ApplicationFrame.Size);
			try {
				var mainLayer = this.MainView.Layer;
				mainLayer.RenderInContext (UIGraphics.GetCurrentContext ());
				var img = UIScreen.MainScreen.Capture ();
				var newImg = UIImage.FromImage (img.CGImage, 1f, UIImageOrientation.Left);
				newImg.SaveToPhotosAlbum ((iRef, status) => {
					if (status != null) {
						new UIAlertView ("Problem", status.ToString (), null, "OK", null).Show ();
					} else {
						new UIAlertView ("Screenshot", "Screenshot is Saved", null, "OK", null).Show ();
					}
				});
			} finally {
				UIGraphics.EndImageContext ();
			}
		}

		public void DismissSettingsViewController ()
		{

			settingsViewController.DismissViewController (true, null);
		}

		public void DismissMeasurementsViewController ()
		{
			/*var test = MarkerUnit.dt;


			if (MeasurementsViewController.isMarkerMeasurement) {
				if (MeasurementsViewController.SelectedMeasurement != "Enable/Disable Markers")
					test = (MarkerUnit)Enum.Parse (typeof(MarkerUnit), MeasurementsViewController.SelectedMeasurement, true);
				else
					test = MarkerUnit.EnableDisableMarkers; 

				markerMeasurements [MeasurementsViewController.SelectedChannel].SelectedUnit = test;
			}
			else
				signalMeasurements [MeasurementsViewController.SelectedChannel].SelectedUnit = (SignalUnit)Enum.Parse (typeof(SignalUnit), MeasurementsViewController.SelectedMeasurement, true);

			measurementsViewController.DismissViewController (true, null);
			UpdateScopeControls ();*/
		}

		#endregion


		private void UpdateScopeControls ()
		{
			UpdateSelectedChannel ();
			UpdateChannel1UI ();
			UpdateChannel2UI ();
			UpdateTriggerUI ();
			UpdateMeasurements ();
		}


		void UpdateChannel1UI ()
		{
			UpdateInputCoupling1 ();
			UpdateAttenuationFactor1 ();
			UpdateVoltText1 ();
		}

		void UpdateChannel2UI ()
		{
			UpdateInputCoupling2 ();
			UpdateAttenuationFactor2 ();
			UpdateVoltText2 ();
		}

		void UpdateTriggerUI ()
		{
			UpdateTriggerChannelUI ();
			UpdateTriggerSlopeUI ();
			UpdateTriggerModeUI ();
			UpdateHoldUI ();
			UpdateAutorangeUI ();
			UpdateTimeBaseText ();
		}

		protected void UpdateMeasurements ()
		{
			// Channel 1
			UpdateMarkerMeasurement1 ();
			btnSignalMeasurements.SetTitle (GetMeasurementString (DisplaySettings.SignalUnits [0], 0), UIControlState.Normal);

			// Channel 2
			UpdateMarkerMeasurement2 ();
			btnSignalMeasurements2.SetTitle (GetMeasurementString (DisplaySettings.SignalUnits [1], 1), UIControlState.Normal);
		}

		void UpdateSelectedChannel ()
		{
			if (ScopeView.SelectedChannel == 0) {
				btnSelectChannel1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-ON-6x6.png"), UIControlState.Normal);
				btnSelectChannel2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-OFF-6x710.png"), UIControlState.Normal);
			} else {
				btnSelectChannel1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-OFF-6x6.png"), UIControlState.Normal);
				btnSelectChannel2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-ON-6x710.png"), UIControlState.Normal);
			}
		}

		void UpdateInputCoupling1 ()
		{
			switch (Oscilloscope.Channels [0].InputCoupling) {
			case InputCoupling.AC:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-ON-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6.png"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-ON-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6.png"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6.png"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6.png"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-ON-263x6.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor1 ()
		{
			switch (Oscilloscope.Channels [0].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-1X-OFF-344x6.png"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-10X-ON-344x6.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateVoltText1 ()
		{
			lblVolt1.Text = VoltsPerDivisionConverter.ToString (Oscilloscope.Channels [0].VoltsPerDivision);
		}

		void UpdateInputCoupling2 ()
		{
			switch (Oscilloscope.Channels [1].InputCoupling) {
			case InputCoupling.AC:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-ON-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710.png"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-ON-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710.png"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710.png"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710.png"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-ON-263x710.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor2 ()
		{
			switch (Oscilloscope.Channels [1].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-1X-OFF-344x710.png"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-10X-ON-344x710.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateVoltText2 ()
		{
			lblVolt2.Text = VoltsPerDivisionConverter.ToString (Oscilloscope.Channels [1].VoltsPerDivision);
		}

		void UpdateTriggerChannelUI ()
		{
			if (Oscilloscope.Trigger.Channel == 0) {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN1-ON-6x96.png"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN2-OFF-60x96.png"), UIControlState.Normal);
			} else {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN1-OFF-6x96.png"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN2-ON-60x96.png"), UIControlState.Normal);
			}
		}

		void UpdateTriggerSlopeUI ()
		{
			if (Oscilloscope.Trigger.Slope == TriggerSlope.Rising) {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-UP-ON-6x156.png"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-DOWN-OFF-60x156.png"), UIControlState.Normal);
			} else {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-UP-OFF-6x156.png"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-DOWN-ON-60x156.png"), UIControlState.Normal);
			}
		}

		void UpdateTriggerModeUI ()
		{
			switch (Oscilloscope.Trigger.Mode) {
			case TriggerMode.Normal:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-ON-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216.png"), UIControlState.Normal);
				break;
			case TriggerMode.Once:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-ON-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216.png"), UIControlState.Normal);
				break;
			case TriggerMode.Run:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276.png"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276.png"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-ON-6x216.png"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateHoldUI ()
		{
			if (Oscilloscope.Hold)
				btnTriggerHold.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-HOLD-ON-6x336.png"), UIControlState.Normal);
			else
				btnTriggerHold.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-HOLD-OFF-6x336.png"), UIControlState.Normal);
		}

		void UpdateAutorangeUI ()
		{
			if (Oscilloscope.AutoRange)
				btnAutorange.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/AUTO RANGE/AUTORANGE-ON-6x541.png"), UIControlState.Normal);
			else
				btnAutorange.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/AUTO RANGE/AUTORANGE-OFF-6x541.png"), UIControlState.Normal);
		}

		void UpdateTimeBaseText ()
		{
			lblTime.Text = TimeBaseConverter.ToString (Oscilloscope.TimeBase);
		}

		private void SetSignalWithDtValue (int channel)
		{
			var value = MarkerDataCalculator.CalculateTime (Oscilloscope.TimeBase, ScopeView.xMarkers [0].Value, ScopeView.xMarkers [1].Value, Oscilloscope.DeviceContext, ScopeView.Frame);
			value = Math.Round (value, 6);
			var title = ToEngineeringNotation (value);
			title += "s";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);
		}

		void SetSignalWithFrequency (int channel)
		{
			var value = MarkerDataCalculator.CalculateFrequency (Oscilloscope.TimeBase, ScopeView.xMarkers [0].Value, ScopeView.xMarkers [1].Value, Oscilloscope.DeviceContext, ScopeView.Frame);
			value = Math.Round (value, 2);
			var title = ToEngineeringNotation (value);
			title += "Hz";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);
		}



		void SetSignalWithDV1 (int channel)
		{
			var value = MarkerDataCalculator.CalculateDV (Oscilloscope.Channels [0].VoltsPerDivision, ScopeView.yMarkers [0].Value, ScopeView.yMarkers [1].Value, Oscilloscope.DeviceContext, ScopeView.Frame);
			value = Math.Round (value, 2);
			var title = ToEngineeringNotation (value);
			title += "V";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);
		}

		void SetSignalWithDV2 (int channel)
		{
			var value = MarkerDataCalculator.CalculateDV (Oscilloscope.Channels [1].VoltsPerDivision, ScopeView.yMarkers [0].Value, ScopeView.yMarkers [1].Value, Oscilloscope.DeviceContext, ScopeView.Frame);
			value = Math.Round (value, 2);
			var title = ToEngineeringNotation (value);
			title += "V";
			if (channel == 0)
				btnMarkerMeasurements.SetTitle (title, UIControlState.Normal);
			else
				btnMarkerMeasurements2.SetTitle (title, UIControlState.Normal);

		}

		void EnableDisableMarkers ()
		{
			ScopeView.ToggleMarkers ();
		}

		void UpdateMarkerMeasurement1 ()
		{
			switch (DisplaySettings.MarkerUnits [0]) {
			case MarkerUnit.dt:
				SetSignalWithDtValue (0);
				break;
			case MarkerUnit.Frequency:
				SetSignalWithFrequency (0);
				break;
			case MarkerUnit.dV1:
				SetSignalWithDV1 (0);
				break;
			case MarkerUnit.dV2:
				SetSignalWithDV2 (0);
				break;
			case MarkerUnit.EnableDisableMarkers:
				EnableDisableMarkers ();
				DisplaySettings.MarkerUnits [0] = MarkerUnit.dt;
				break;
			default:
				btnMarkerMeasurements.SetTitle ("Unsupported Measurement", UIControlState.Normal);
				break;
			}
		}

		void UpdateMarkerMeasurement2 ()
		{
			switch (DisplaySettings.MarkerUnits [1]) {
			case MarkerUnit.dt:
				SetSignalWithDtValue (1);
				break;
			case MarkerUnit.Frequency:
				SetSignalWithFrequency (1);
				break;
			case MarkerUnit.dV1:
				SetSignalWithDV1 (1);
				break;
			case MarkerUnit.dV2:
				SetSignalWithDV2 (1);
				break;
			case MarkerUnit.EnableDisableMarkers:
				EnableDisableMarkers ();
				DisplaySettings.MarkerUnits [1] = MarkerUnit.dt;
				break;
			default:
				btnMarkerMeasurements.SetTitle ("Unsupported Measurement", UIControlState.Normal);
				break;
			}
		}

		public string GetMeasurementString (SignalUnit unit, int channel)
		{
			switch (unit) {
			case SignalUnit.DbGain:
				return VoltageConverter.ToString (Oscilloscope.DBGain ());
			case SignalUnit.Dbm1:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].DBm ());
			case SignalUnit.Dbm2:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].DBm ());
			case SignalUnit.RMS:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Vrms ());
			case SignalUnit.TRMS:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].VTrms ());
			case SignalUnit.Vdc:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Vdc ());
			case SignalUnit.VMax:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Vmax ());
			case SignalUnit.VMin:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Vmin ());
			case SignalUnit.Vptp:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Vptp ());
			case SignalUnit.WRMS16:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Wrms16 ());
			case SignalUnit.WRMS2:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Wrms2 ());
			case SignalUnit.WRMS32:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Wrms32 ());
			case SignalUnit.WRMS4:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Wrms4 ());
			case SignalUnit.WRMS8:
				return VoltageConverter.ToString (Oscilloscope.Channels [channel].Wrms8 ());
			default:
				return "?";
			}
		}

		private static string ToEngineeringNotation (double d)
		{
			double exponent = Math.Log10 (Math.Abs (d));
			if (Math.Abs (d) >= 1) {
				switch ((int)Math.Floor (exponent)) {
				case 0:
				case 1:
				case 2:
					return d.ToString ();
				case 3:
				case 4:
				case 5:
					return (d / 1e3).ToString () + "k";
				case 6:
				case 7:
				case 8:
					return (d / 1e6).ToString () + "M";
				case 9:
				case 10:
				case 11:
				default:
					return (d / 1e24).ToString () + "Y";
				}
			} else if (Math.Abs (d) > 0) {
				switch ((int)Math.Floor (exponent)) {
				case -1:
				case -2:
				case -3:
					return (d * 1e3).ToString () + "m";
				case -4:
				case -5:
				case -6:
					return (d * 1e6).ToString () + "μ";
				case -7:
				case -8:
				case -9:
					return (d * 1e9).ToString () + "n";
				default:
					return (d * 1e15).ToString () + "y";
				}
			} else {
				return "0";
			}
		}
	}
}
