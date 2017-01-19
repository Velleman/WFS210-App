using System;
using WFS210;
using System.Timers;
using WFS210.Services;
using Foundation;
using System.Drawing;
using UIKit;
using System.Net.NetworkInformation;
using System.Linq;

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


		/// <summary>
		/// The detail view popover.
		/// </summary>
		UIPopoverController DetailViewPopover;

		/// <summary>
		/// Initializes a new instance of the <see cref="iWFS210ViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public iWFS210ViewController (IntPtr handle) : base (handle)
		{
			Oscilloscope = new Oscilloscope();
			var markerExist = NSUserDefaults.StandardUserDefaults["markers"];
			if (markerExist == null)
			{
				NSUserDefaults.StandardUserDefaults.SetBool(true, "markers");
			}
			var demo = NSUserDefaults.StandardUserDefaults.BoolForKey ("demo");
			if (demo){
				ServiceManager = new ServiceManager(Oscilloscope, ServiceType.Demo);
			}
			else {
				ServiceManager = new ServiceManager(Oscilloscope, ServiceType.Live);
				GetIpAdress();
				if (!Service.Activate()) {
					ServiceManager.ServiceType = ServiceType.Demo;
					new UIAlertView ("No Connection", "Demo mode is now running", null, "OK", null).Show ();
				} else {
					Service.RequestSettings ();
					Service.RequestWifiSettings ();
					Service.RequestSamples ();
				}
			}
			DisplaySettings = new DisplaySettings (MarkerUnit.dt, SignalUnit.Vdc);
		}

		/// <summary>
		/// When Settings are changed update the controls and the signal;
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void SettingsChanged (object sender, EventArgs e)
		{
			InvokeOnMainThread (() => {
				UpdateScopeControls ();
				ScopeView.Update ();
			});
			if (!Oscilloscope.Hold && Oscilloscope.Trigger.Mode == TriggerMode.Run) {
				Oscilloscope.Channels [0].Samples.Clear ();
				Oscilloscope.Channels [1].Samples.Clear ();
			}
		}

		void GetIpAdress()
		{
			var cards = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var card in cards)
			{
				if (card.Name == "en0")
				{
					var address = card.GetIPProperties().GatewayAddresses.FirstOrDefault();
					var liveService = ServiceManager.GetService(ServiceType.Live) as LiveService;
					if (address != null)
					{
						liveService.Connection.IPAddress = address.Address.ToString();
					}

				}
			}
		}

		#region View lifecycle

		/// <summary>
		/// Views did load.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			MainView.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("BACKGROUND/BG-0x0"));
			ServiceManager.SettingsChanged += SettingsChanged;
			ServiceManager.FullFrame += HandleFullFrame;
			ScopeView.ServiceManager = ServiceManager;
			ScopeView.Initialize ();
			var timer = new Timer(200);
			timer.Elapsed += (object sender, ElapsedEventArgs e) => {
				timer.Stop();
				Service.Update ();
				InvokeOnMainThread (ScopeView.Update);
				timer.Start();
			};
			timer.Enabled = true;
			timer.Start ();

			ScopeView.SelectedChannel = 0;

			ScopeView.NewData += (object sender, NewDataEventArgs e) => InvokeOnMainThread (UpdateMeasurements);

		}

		void HandleFullFrame (object sender, EventArgs e)
		{
			InvokeOnMainThread (UpdateMeasurements);
		}

		/// <summary>
		/// Views did appear.
		/// </summary>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			UpdateScopeControls ();
			UIApplication.SharedApplication.IdleTimerDisabled = true;
			var currentVersion = new Version(UIDevice.CurrentDevice.SystemVersion);
			if (currentVersion.Major >= 10)
			{
				if (!NSUserDefaults.StandardUserDefaults.BoolForKey(new NSString("IOS10ERRORSHOWN")))
				{
					ShowIOS10Fault();
					NSUserDefaults.StandardUserDefaults.SetBool(true, "IOS10ERRORSHOWN");
				}
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			UIApplication.SharedApplication.IdleTimerDisabled = false;
		}

		/// <Docs>Whether this UIViewController prefers the status bar to be hidden.</Docs>
		/// <remarks>To be added.</remarks>
		/// <summary>
		/// Preferses the status bar hidden.
		/// </summary>
		/// <returns><c>true</c>, if status bar hidden was prefersed, <c>false</c> otherwise.</returns>
		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}

		#endregion

		private void ShowIOS10Fault()
		{
			var alert = UIAlertController.Create("No Conection", "It seems your device is using iOS10 or higher, please update your WFS210 with the new firmware. Visit www.velleman.eu for more info", UIAlertControllerStyle.Alert);

			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));

			PresentViewController(alert, true, null);
		}


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
			InvokeOnMainThread (() => ShowMarkerUnitPopover (sender, 0));
		}

		partial void btnSignalMeasurements_TouchUpInside (UIButton sender)
		{
			InvokeOnMainThread (() => ShowSignalUnitPopover (sender, 0));
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
			if (!Oscilloscope.AutoRange) {
				if (Service.Oscilloscope.Channels [0].VoltsPerDivision == VoltsPerDivision.VdivNone) {
					Service.Oscilloscope.Channels [0].VoltsPerDivision = VoltsPerDivision.Vdiv20V;
				}
				if (Service.Oscilloscope.Channels [1].VoltsPerDivision == VoltsPerDivision.VdivNone) {
					Service.Oscilloscope.Channels [1].VoltsPerDivision = VoltsPerDivision.Vdiv20V;
				}
				Service.ApplySettings();
				Oscilloscope.AutoRange = !Oscilloscope.AutoRange;
			}
			else
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
				DisplaySettings.SignalUnits [channel] = e.Value;
			};

			DetailViewPopover = new UIPopoverController (content);
			DetailViewPopover.PopoverContentSize = content.ContentBounds.Size;

			DetailViewPopover.PresentFromRect (view.Frame, View, UIPopoverArrowDirection.Any, true);
		}

		private void ShowMarkerUnitPopover (UIView view, int channel)
		{
			var content = new PopoverContentViewController<MarkerUnit> ();
			content.ValueChanged += (object s, EnumEventArgs<MarkerUnit> e) => {
				DisplaySettings.MarkerUnits [channel] = e.Value;
			};

			DetailViewPopover = new UIPopoverController (content);
			DetailViewPopover.PopoverContentSize = content.ContentBounds.Size;

			DetailViewPopover.PresentFromRect (view.Frame, View, UIPopoverArrowDirection.Any, true);
		}

		#endregion

		#region Events Settings

		partial void btnSettings_TouchUpInside (UIButton sender)
		{
			settingsViewController = Storyboard.InstantiateViewController("SettingsViewController") as SettingsViewController;
			settingsViewController.WifiSetting = Oscilloscope.WifiSetting;
			settingsViewController.ServiceManager = ServiceManager;
			settingsViewController.RequestedDismiss += (object s, EventArgs de) => {
				settingsViewController.DismissViewController (true, null);
				ScopeView.MarkersAreVisible = NSUserDefaults.StandardUserDefaults.BoolForKey ("markers");
			};
			settingsViewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
			PresentViewController (settingsViewController, true, null);
		}

		partial void btnSnap_TouchUpInside (UIButton sender)
		{
			UIGraphics.BeginImageContext (UIScreen.MainScreen.ApplicationFrame.Size);
			try {
				var mainLayer = MainView.Layer;
				mainLayer.RenderInContext (UIGraphics.GetCurrentContext ());
				var orientation = UIApplication.SharedApplication.StatusBarOrientation;
				var img = UIScreen.MainScreen.Capture ();
				UIImage screenshot = null;
				if (orientation == UIInterfaceOrientation.LandscapeLeft) {
					screenshot = UIImage.FromImage (img.CGImage, 1f, UIImageOrientation.Right);
				} else if (orientation == UIInterfaceOrientation.LandscapeRight) {
					screenshot = UIImage.FromImage (img.CGImage, 1f, UIImageOrientation.Left);
				}
				
				screenshot.SaveToPhotosAlbum ((iRef, status) => {
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

		#endregion


		private void UpdateScopeControls ()
		{
			UpdateSelectedChannel ();
			UpdateChannel1UI ();
			UpdateChannel2UI ();
			UpdateTriggerUI ();
			UpdateMeasurements ();
			UpdateBatteryStatus ();
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
			btnSignalMeasurements.SetTitle (GetMeasurementString (DisplaySettings.SignalUnits [0], 0), UIControlState.Normal);
			btnMarkerMeasurements.SetTitle (GetMarkerMeasurementString (DisplaySettings.MarkerUnits [0]), UIControlState.Normal);


			// Channel 2
			btnSignalMeasurements2.SetTitle (GetMeasurementString (DisplaySettings.SignalUnits [1], 1), UIControlState.Normal);
			btnMarkerMeasurements2.SetTitle (GetMarkerMeasurementString (DisplaySettings.MarkerUnits [1]), UIControlState.Normal);

		}

		void UpdateBatteryStatus ()
		{
			switch (Oscilloscope.BatteryStatus) {
			case BatteryStatus.BatteryLow:
				btnBattery.SetBackgroundImage (UIImage.FromBundle ("BATTERY/BATT-LOW-984x5"), UIControlState.Normal);
				break;
			case BatteryStatus.Charging:
				btnBattery.SetBackgroundImage (UIImage.FromBundle ("BATTERY/BATT-CHARGING-984x5"), UIControlState.Normal);
				break;
			case BatteryStatus.Charged:
				btnBattery.SetBackgroundImage (UIImage.FromBundle ("BATTERY/BATT-FULL-984x5"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateSelectedChannel ()
		{
			if (ScopeView.SelectedChannel == 0) {
				btnSelectChannel1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-ON-6x6"), UIControlState.Normal);
				btnSelectChannel2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-OFF-6x710"), UIControlState.Normal);
			} else {
				btnSelectChannel1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-OFF-6x6"), UIControlState.Normal);
				btnSelectChannel2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-ON-6x710"), UIControlState.Normal);
			}
		}

		void UpdateInputCoupling1 ()
		{
			switch (Oscilloscope.Channels [0].InputCoupling) {
			case InputCoupling.AC:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-ON-129x6"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-ON-196x6"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-OFF-263x6"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-AC-OFF-129x6"), UIControlState.Normal);
				btnDC1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-DC-OFF-196x6"), UIControlState.Normal);
				btnGND1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-GND-ON-263x6"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor1 ()
		{
			switch (Oscilloscope.Channels [0].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-1X-OFF-344x6"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 1/CHAN1-10X-ON-344x6"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateVoltText1 ()
		{
			lblVolt1.Text = VoltsPerDivisionConverter.ToString (Oscilloscope.Channels [0].VoltsPerDivision, Oscilloscope.Channels [0].AttenuationFactor);
		}

		void UpdateInputCoupling2 ()
		{
			switch (Oscilloscope.Channels [1].InputCoupling) {
			case InputCoupling.AC:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-ON-129x710"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710"), UIControlState.Normal);
				break;
			case InputCoupling.DC:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-ON-196x710"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-OFF-263x710"), UIControlState.Normal);
				break;
			case InputCoupling.GND:
				btnAC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-AC-OFF-129x710"), UIControlState.Normal);
				btnDC2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-DC-OFF-196x710"), UIControlState.Normal);
				btnGND2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-GND-ON-263x710"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor2 ()
		{
			switch (Oscilloscope.Channels [1].AttenuationFactor) {
			case AttenuationFactor.X1:
				btnProbe2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-1X-OFF-344x710"), UIControlState.Normal);
				break;
			case AttenuationFactor.X10:
				btnProbe2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/CHANNEL 2/CHAN2-10X-ON-344x710"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateVoltText2 ()
		{
			lblVolt2.Text = VoltsPerDivisionConverter.ToString (Oscilloscope.Channels [1].VoltsPerDivision, Oscilloscope.Channels [1].AttenuationFactor);
		}

		void UpdateTriggerChannelUI ()
		{
			if (Oscilloscope.Trigger.Channel == 0) {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN1-ON-6x96"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN2-OFF-60x96"), UIControlState.Normal);
			} else {
				btnTriggerCH1.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN1-OFF-6x96"), UIControlState.Normal);
				btnTriggerCH2.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-CHAN2-ON-60x96"), UIControlState.Normal);
			}
		}

		void UpdateTriggerSlopeUI ()
		{
			if (Oscilloscope.Trigger.Slope == TriggerSlope.Rising) {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-UP-ON-6x156"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-DOWN-OFF-60x156"), UIControlState.Normal);
			} else {
				btnTriggerSlopeUp.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-UP-OFF-6x156"), UIControlState.Normal);
				btnTriggerSlopeDown.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-DOWN-ON-60x156"), UIControlState.Normal);
			}
		}

		void UpdateTriggerModeUI ()
		{
			switch (Oscilloscope.Trigger.Mode) {
			case TriggerMode.Normal:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-ON-6x276"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216"), UIControlState.Normal);
				break;
			case TriggerMode.Once:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-ON-60x276"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-OFF-6x216"), UIControlState.Normal);
				break;
			case TriggerMode.Run:
				btnTriggerNrml.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-NRML-OFF-6x276"), UIControlState.Normal);
				btnTriggerOnce.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-ONCE-OFF-60x276"), UIControlState.Normal);
				btnTriggerRun.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-RUN-ON-6x216"), UIControlState.Normal);
				break;
			default:
				break;
			}
		}

		void UpdateHoldUI ()
		{
			if (Oscilloscope.Hold)
				btnTriggerHold.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-HOLD-ON-6x336"), UIControlState.Normal);
			else
				btnTriggerHold.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/TRIGGER/TRIG-HOLD-OFF-6x336"), UIControlState.Normal);
		}

		void UpdateAutorangeUI ()
		{
			if (Oscilloscope.AutoRange)
				btnAutorange.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/AUTO RANGE/AUTORANGE-ON-6x541"), UIControlState.Normal);
			else
				btnAutorange.SetBackgroundImage (UIImage.FromBundle ("BUTTONS/AUTO RANGE/AUTORANGE-OFF-6x541"), UIControlState.Normal);
		}

		void UpdateTimeBaseText ()
		{
			lblTime.Text = TimeBaseConverter.ToString (Oscilloscope.TimeBase);
		}

		public string GetMarkerMeasurementString (MarkerUnit unit)
		{
			switch (unit) {
			case MarkerUnit.dt:
				return TimeConverter.ToString (MarkerDataCalculator.CalculateTime (Oscilloscope.TimeBase, ScopeView.XMarkers [0], ScopeView.XMarkers [1], ScopeView.Frame), 2);
			case MarkerUnit.Frequency:
				return FrequencyConverter.ToString (MarkerDataCalculator.CalculateFrequency (Oscilloscope.TimeBase, ScopeView.XMarkers [0], ScopeView.XMarkers [1], ScopeView.Frame));
			case MarkerUnit.dV1:
				return Oscilloscope.Channels [0].VoltsPerDivision == VoltsPerDivision.VdivNone ? "--" : VoltageConverter.ToString (MarkerDataCalculator.CalculateDV (Oscilloscope.Channels [0].VoltsPerDivision, Oscilloscope.Channels [0].AttenuationFactor, ScopeView.YMarkers [0], ScopeView.YMarkers [1], ScopeView.Frame));
			case MarkerUnit.dV2:
				return Oscilloscope.Channels [1].VoltsPerDivision == VoltsPerDivision.VdivNone ? "--" : VoltageConverter.ToString (MarkerDataCalculator.CalculateDV (Oscilloscope.Channels [1].VoltsPerDivision, Oscilloscope.Channels [0].AttenuationFactor, ScopeView.YMarkers [0], ScopeView.YMarkers [1], ScopeView.Frame));
			default:
				return "?";
			}
		}

		public string GetMeasurementString (SignalUnit unit, int channel)
		{
			if (Oscilloscope.Channels [channel].VoltsPerDivision != VoltsPerDivision.VdivNone) {
				if (Oscilloscope.Channels [channel].Samples.Overflow) {
					return "Overflow";
				} else {
					switch (unit) {
					case SignalUnit.dBGain:
						return DecibelConverter.ToString (Oscilloscope.DBGain ());
					case SignalUnit.dBm:
						return DecibelConverter.ToString (Oscilloscope.Channels [channel].DBm ());
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
						return WattConverter.ToString (Oscilloscope.Channels [channel].Wrms16 ());
					case SignalUnit.WRMS2:
						return WattConverter.ToString (Oscilloscope.Channels [channel].Wrms2 ());
					case SignalUnit.WRMS32:
						return WattConverter.ToString (Oscilloscope.Channels [channel].Wrms32 ());
					case SignalUnit.WRMS4:
						return WattConverter.ToString (Oscilloscope.Channels [channel].Wrms4 ());
					case SignalUnit.WRMS8:
						return WattConverter.ToString (Oscilloscope.Channels [channel].Wrms8 ());
					default:
						return "?";
					}
				}
			} else {
				return "--";
			}
		}
	}
}
