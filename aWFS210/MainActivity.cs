using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using WFS210;
using WFS210.Services;


namespace aWFS210
{
	[Activity (Label = "aWFS210", MainLauncher = true, Icon = "@drawable/icon", Theme="@android:style/Theme.Black.NoTitleBar.Fullscreen")]
	public class MainActivity : Activity
	{
		/// <summary>
		/// The oscilloscope.
		/// </summary>
		protected Oscilloscope Oscilloscope;

		/// <summary>
		/// The service manager.
		/// </summary>
		protected ServiceManager ServiceManager;

		/// <summary>
		/// The scope view.
		/// </summary>
		private ScopeView ScopeView;

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <value>The service.</value>
		public WFS210.Services.Service Service {
			get { return ServiceManager.ActiveService; }
		}

		private Button btnSelectChannel1, btnSelectChannel2,
			btnTriggerCH1, btnTriggerCH2,
			btnSlopeUp, btnSlopeDown,
			btnRun, btnNrml, btnOnce, btnHold,
			btnTimebaseLeft, btnTimebaseRight,
			btnAutoRange,
			btnAC1, btnAC2,
			btnDC1, btnDC2,
			btnGND1, btnGND2,
			btnProbe1, btnProbe2,
			btnVoltUp1, btnVoltUp2,
			btnVoltDown1, btnVoltDown2;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Create the Oscilloscope
			this.Oscilloscope = new Oscilloscope ();

			//Create the ServiceManager
			this.ServiceManager = new ServiceManager (Oscilloscope, ServiceType.Demo);

			this.ScopeView = FindViewById<ScopeView> (Resource.Id.ScopeView);

			LoadControls();

		}

		protected override void OnResume ()
		{
			base.OnResume ();
			ServiceManager.SettingsChanged += HandleSettingsChanged;
			ServiceManager.FullFrame += HandleFullFrame;
			this.ScopeView.ServiceManager = this.ServiceManager;
			this.ScopeView.Oscilloscope = this.Oscilloscope;
			this.ScopeView.Initialize ();
			var timer = new System.Timers.Timer (200);
			timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
				timer.Stop();
				Service.Update ();
				RunOnUiThread (() =>ScopeView.Update());
				timer.Start();
			};
			timer.Enabled = true;
			timer.Start ();

			ScopeView.SelectedChannel = 0;

			ScopeView.NewData += (object sender, NewDataEventArgs e) => RunOnUiThread (() => UpdateMeasurements());
		}

		void HandleFullFrame (object sender, EventArgs e)
		{
			RunOnUiThread (() => UpdateMeasurements());
		}

		void HandleSettingsChanged (object sender, EventArgs e)
		{
			RunOnUiThread (() => {
				UpdateScopeControls ();
				ScopeView.Update ();
			});
			if (!Oscilloscope.Hold && Oscilloscope.Trigger.Mode == TriggerMode.Run) {
				Oscilloscope.Channels [0].Samples.Clear ();
				Oscilloscope.Channels [1].Samples.Clear ();
			}
		}

		private void UpdateScopeControls ()
		{
//			UpdateSelectedChannel ();
//			UpdateChannel1UI ();
//			UpdateChannel2UI ();
//			UpdateTriggerUI ();
//			UpdateMeasurements ();
//			UpdateBatteryStatus ();
		}


		private void UpdateChannel1UI ()
		{
//			UpdateInputCoupling1 ();
//			UpdateAttenuationFactor1 ();
//			UpdateVoltText1 ();
		}

		private void UpdateChannel2UI ()
		{
//			UpdateInputCoupling2 ();
//			UpdateAttenuationFactor2 ();
//			UpdateVoltText2 ();
		}

		private void UpdateTriggerUI ()
		{
//			UpdateTriggerChannelUI ();
//			UpdateTriggerSlopeUI ();
//			UpdateTriggerModeUI ();
//			UpdateHoldUI ();
//			UpdateAutorangeUI ();
//			UpdateTimeBaseText ();
		}


		private void UpdateMeasurements ()
		{
//			// Channel 1
//			btnSignalMeasurements.SetTitle (GetMeasurementString (DisplaySettings.SignalUnits [0], 0), UIControlState.Normal);
//			btnMarkerMeasurements.SetTitle (GetMarkerMeasurementString (DisplaySettings.MarkerUnits [0]), UIControlState.Normal);
//
//
//			// Channel 2
//			btnSignalMeasurements2.SetTitle (GetMeasurementString (DisplaySettings.SignalUnits [1], 1), UIControlState.Normal);
//			btnMarkerMeasurements2.SetTitle (GetMarkerMeasurementString (DisplaySettings.MarkerUnits [1]), UIControlState.Normal);

		}

		private void LoadControls()
		{
			//Channel1 Select
			btnSelectChannel1 = FindViewById<Button> (Resource.Id.btnCH1);
			btnSelectChannel1.Click += HandleCH1Click;

			btnSelectChannel2 = FindViewById<Button> (Resource.Id.btnCH2);
			btnSelectChannel2.Click += HandleCH2Click;

			btnAC1 = FindViewById<Button> (Resource.Id.btnAC1);
			btnAC1.Click += HandleAC1Click;

			btnAC2 = FindViewById<Button> (Resource.Id.btnAC2);
			btnAC2.Click += HandleAC2Click;

			btnDC1 = FindViewById<Button>(Resource.Id.btnDC1);
			btnDC1.Click += HandleDC1Click;

			btnDC2 = FindViewById<Button>(Resource.Id.btnDC2);
			btnDC2.Click += HandleDC2Click;

			btnGND1 = FindViewById<Button>(Resource.Id.btnGND1);
			btnGND1.Click += HandleGND1Click;

			btnGND2 = FindViewById<Button>(Resource.Id.btnGND2);
			btnGND2.Click += HandleGND2Click;

			btnProbe1 = FindViewById<Button>(Resource.Id.btnPROBE1);
			btnProbe1.Click += HandleProbe1Click;

			btnProbe2 = FindViewById<Button>(Resource.Id.btnPROBE2);
			btnProbe2.Click += HandleProbe2Click;

			btnVoltDown1 = FindViewById<Button> (Resource.Id.btnVoltDown1);
			btnVoltDown1.Click += HandleVoltDown1Click;

			btnVoltDown2 = FindViewById<Button> (Resource.Id.btnVoltDown2);
			btnVoltDown2.Click += HandleVoltDown2Click;

			btnVoltUp1 = FindViewById<Button> (Resource.Id.btnVoltUp1);
			btnVoltUp1.Click += HandleVoltUp1Click;

			btnVoltUp2 = FindViewById<Button> (Resource.Id.btnVoltUp2);
			btnVoltUp2.Click += HandleVoltUp2;

			btnTriggerCH1 = FindViewById<Button> (Resource.Id.btnTriggerCh1);
			btnTriggerCH1.Click += HandleTriggerCH1CLick;

			btnTriggerCH2 = FindViewById<Button> (Resource.Id.btnTriggerCh2);
			btnTriggerCH2.Click += HandleTriggerCH2Click;

			btnSlopeUp = FindViewById<Button> (Resource.Id.btnRising);
			btnSlopeUp.Click += HandleSlopeUpClick;

			btnSlopeDown = FindViewById<Button> (Resource.Id.btnFalling);
			btnSlopeDown.Click += HandleSlopeDownClick;

			btnRun = FindViewById<Button> (Resource.Id.btnRun);
			btnRun.Click += HandleRunClick;

			btnNrml = FindViewById<Button> (Resource.Id.btnNormal);
			btnNrml.Click += HandleNrmlClick;

			btnOnce = FindViewById<Button> (Resource.Id.btnOnce);
			btnOnce.Click += HandleOnceClick;

			btnHold = FindViewById<Button> (Resource.Id.btnHold);
			btnHold.Click += HandleHoldClick;

			btnTimebaseLeft = FindViewById<Button> (Resource.Id.btnTimeBaseLeft);
			btnTimebaseLeft.Click += HandleTimebaseLeftClick;

			btnTimebaseRight = FindViewById<Button> (Resource.Id.btnTimeBaseRight);
			btnTimebaseRight.Click += HandleTimebaseRightClick;

			btnAutoRange = FindViewById<Button> (Resource.Id.btnAutoRange);
			btnAutoRange.Click += HandleAutorangeClick;
		}

		void HandleAutorangeClick (object sender, EventArgs e)
		{
			if (!Service.Oscilloscope.AutoRange) {
				if (Service.Oscilloscope.Channels [0].VoltsPerDivision == VoltsPerDivision.VdivNone) {
					Service.Oscilloscope.Channels [0].VoltsPerDivision = VoltsPerDivision.Vdiv20V;
				}
				if (Service.Oscilloscope.Channels [1].VoltsPerDivision == VoltsPerDivision.VdivNone) {
					Service.Oscilloscope.Channels [1].VoltsPerDivision = VoltsPerDivision.Vdiv20V;
				}
				Service.ApplySettings ();
				Service.Oscilloscope.AutoRange = !Service.Oscilloscope.AutoRange;
			} else {
				Service.Oscilloscope.AutoRange = !Service.Oscilloscope.AutoRange;
			}
			Service.ApplySettings ();
		}

		void HandleTimebaseRightClick(object sender, EventArgs e)
		{
			Service.Execute (new NextTimeBaseCommand ());
		}

		void HandleTimebaseLeftClick (object sender, EventArgs e)
		{
			Service.Execute (new PreviousTimeBaseCommand ());
		}

		void HandleHoldClick (object sender, EventArgs e)
		{
			Service.Execute (new ToggleHoldCommand ());
		}

		void HandleOnceClick (object sender, EventArgs e)
		{
			Service.Execute (new TriggerModeCommand (TriggerMode.Once));
		}

		void HandleNrmlClick (object sender, EventArgs e)
		{
			Service.Execute (new TriggerModeCommand (TriggerMode.Normal));
		}

		void HandleRunClick (object sender, EventArgs e)
		{
			Service.Execute (new TriggerModeCommand (TriggerMode.Run));
		}

		void HandleSlopeDownClick (object sender, EventArgs e)
		{
			Service.Execute (new TriggerSlopeCommand (TriggerSlope.Falling));
		}

		void HandleSlopeUpClick (object sender, EventArgs e)
		{
			Service.Execute (new TriggerSlopeCommand (TriggerSlope.Rising));
		}

		void HandleTriggerCH2Click (object sender, EventArgs e)
		{
			Service.Execute (new TriggerChannelCommand (1));
		}

		void HandleTriggerCH1CLick (object sender, EventArgs e)
		{
			Service.Execute (new TriggerChannelCommand (0));
		}

		void HandleVoltUp2 (object sender, EventArgs e)
		{
			Service.Execute (new NextVoltsPerDivisionCommand (1));
		}

		void HandleVoltUp1Click (object sender, EventArgs e)
		{
			Service.Execute (new NextVoltsPerDivisionCommand (0));
		}

		void HandleVoltDown2Click (object sender, EventArgs e)
		{
			Service.Execute (new PreviousVoltsPerDivisionCommand (1));
		}

		void HandleVoltDown1Click (object sender, EventArgs e)
		{
			Service.Execute(new PreviousVoltsPerDivisionCommand(0));
		}

		void HandleProbe2Click (object sender, EventArgs e)
		{
			Service.Execute (new FlipAttenuationFactorCommand (1));
		}

		void HandleProbe1Click (object sender, EventArgs e)
		{
			Service.Execute (new FlipAttenuationFactorCommand (0));
		}

		void HandleGND2Click (object sender, EventArgs e)
		{
			Service.Execute (new InputCouplingCommand (1, InputCoupling.GND));
		}

		void HandleGND1Click (object sender, EventArgs e)
		{
			Service.Execute (new InputCouplingCommand (0, InputCoupling.GND));
		}

		void HandleDC2Click (object sender, EventArgs e)
		{
			Service.Execute (new InputCouplingCommand (1, InputCoupling.DC));
		}

		void HandleDC1Click (object sender, EventArgs e)
		{
			Service.Execute (new InputCouplingCommand (0, InputCoupling.DC));
		}

		void HandleCH2Click (object sender, EventArgs e)
		{
			ScopeView.SelectedChannel = 1;
			UpdateScopeControls ();
		}

		void HandleCH1Click (object sender, EventArgs e)
		{
			ScopeView.SelectedChannel = 0;
			UpdateScopeControls ();
		}

		void HandleAC2Click (object sender, EventArgs e)
		{
			Service.Execute (new InputCouplingCommand (1, InputCoupling.AC));
		}

		void HandleAC1Click (object sender, EventArgs e)
		{
			Service.Execute (new InputCouplingCommand (0, InputCoupling.AC));
		}

	}
}


