using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using WFS210;
using WFS210.Services;
using Android.Graphics.Drawables;
using Android.Content.PM;



namespace WFS210.Android
{
	[Activity (Label = "WFS210", MainLauncher = true, Icon = "@drawable/icon",ScreenOrientation = ScreenOrientation.SensorLandscape, Theme="@android:style/Theme.Black.NoTitleBar.Fullscreen")]
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
		/// The scope data manager.
		/// </summary>
		private SignalView _ScopeView;

		/// <summary>
		/// The Volt Time Indicator view to display the volt/time settings when scaling
		/// </summary>
		private TextView _VoltTimeIndicator;

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
		private TextView lblVolt1,lblVolt2,lblTimebase;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Create the Oscilloscope
			this.Oscilloscope = new Oscilloscope ();

			//Create the ServiceManager
			this.ServiceManager = new ServiceManager (Oscilloscope, ServiceType.Demo);

			this._ScopeView = FindViewById<SignalView> (Resource.Id.SignalView);
			this._VoltTimeIndicator = FindViewById<TextView> (Resource.Id.VoltTimeIndicator);
			this._VoltTimeIndicator.SetZ (999f);
			LoadControls();

		}

		protected override void OnResume ()
		{
			base.OnResume ();
			ServiceManager.SettingsChanged += HandleSettingsChanged;
			ServiceManager.FullFrame += HandleFullFrame;
			this._ScopeView.ServiceManager = this.ServiceManager;
			this._ScopeView.Oscilloscope = this.Oscilloscope;
			var timer = new System.Timers.Timer (200);
			timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
				timer.Stop();
				Service.Update ();
				RunOnUiThread (() =>_ScopeView.Update());
				timer.Start();
			};
			timer.Enabled = true;
			timer.Start ();

			_ScopeView.SelectedChannel = 0;

			_ScopeView.NewData += (object sender, NewDataEventArgs e) => RunOnUiThread (() => UpdateMeasurements());
			UpdateScopeControls ();
		}

		void HandleFullFrame (object sender, EventArgs e)
		{
			RunOnUiThread (() => UpdateMeasurements());
		}

		void HandleSettingsChanged (object sender, EventArgs e)
		{
			RunOnUiThread (() => {
				UpdateScopeControls ();
				_ScopeView.Update ();
			});
			if (!Oscilloscope.Hold && Oscilloscope.Trigger.Mode == TriggerMode.Run) {
				Oscilloscope.Channels [0].Samples.Clear ();
				Oscilloscope.Channels [1].Samples.Clear ();
			}
		}

		private void UpdateScopeControls ()
		{
			UpdateSelectedChannel ();
			if (_ScopeView.IsScaling) {
					_VoltTimeIndicator.Visibility = ViewStates.Visible;
				UpdateVoltTimeIndicator ();
			} else {
				_VoltTimeIndicator.Visibility = ViewStates.Gone;
			}
			UpdateChannel1UI ();
			UpdateChannel2UI ();
			UpdateTriggerUI ();
//			UpdateMeasurements ();
//			UpdateBatteryStatus ();
		}

		private void UpdateVoltTimeIndicator()
		{
			if (_ScopeView.ScalingTime)
				_VoltTimeIndicator.Text = TimeBaseConverter.ToString (Oscilloscope.TimeBase);
			else {
				_VoltTimeIndicator.Text = VoltsPerDivisionConverter.ToString(Oscilloscope.Channels [_ScopeView.SelectedChannel].VoltsPerDivision,Oscilloscope.Channels [_ScopeView.SelectedChannel].AttenuationFactor);
			}
		}

		private void UpdateChannel1UI ()
		{
			UpdateInputCoupling1 ();
			UpdateAttenuationFactor1 ();
			UpdateVoltText1 ();
		}

		private void UpdateChannel2UI ()
		{
			UpdateInputCoupling2 ();
			UpdateAttenuationFactor2 ();
			UpdateVoltText2 ();
		}

		private void UpdateTriggerUI ()
		{
			UpdateTriggerChannelUI ();
			UpdateTriggerSlopeUI ();
			UpdateTriggerModeUI ();
			UpdateHoldUI ();
			UpdateAutorangeUI ();
			UpdateTimeBaseText ();
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

		private void UpdateSelectedChannel ()
		{
			if (_ScopeView.SelectedChannel == 0) {
				SetBackgroundResourceAndKeepPadding(btnSelectChannel1,Resource.Drawable.buttongreen);
				SetBackgroundResourceAndKeepPadding(btnSelectChannel2,Resource.Drawable.button);
			} else {
				SetBackgroundResourceAndKeepPadding(btnSelectChannel1,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnSelectChannel2,Resource.Drawable.buttonyellow);
			}
		}

		void UpdateInputCoupling1 ()
		{
			switch (Oscilloscope.Channels [0].InputCoupling) {
			case InputCoupling.AC:
				SetBackgroundResourceAndKeepPadding(btnAC1,Resource.Drawable.buttongreen);
				SetBackgroundResourceAndKeepPadding(btnDC1,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnGND1,Resource.Drawable.button);
				break;
			case InputCoupling.DC:
				SetBackgroundResourceAndKeepPadding(btnAC1,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnDC1,Resource.Drawable.buttongreen);
				SetBackgroundResourceAndKeepPadding(btnGND1,Resource.Drawable.button);
				break;
			case InputCoupling.GND:
				SetBackgroundResourceAndKeepPadding(btnAC1,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnDC1,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnGND1,Resource.Drawable.buttongreen);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor1 ()
		{
			switch (Oscilloscope.Channels [0].AttenuationFactor) {
			case AttenuationFactor.X1:
				SetBackgroundResourceAndKeepPadding(btnProbe1,Resource.Drawable.button);
				break;
			case AttenuationFactor.X10:
				SetBackgroundResourceAndKeepPadding(btnProbe1,Resource.Drawable.buttongreen);
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
				SetBackgroundResourceAndKeepPadding(btnAC2,Resource.Drawable.buttonyellow);
				SetBackgroundResourceAndKeepPadding(btnDC2,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnGND2,Resource.Drawable.button);
				break;
			case InputCoupling.DC:
				SetBackgroundResourceAndKeepPadding(btnAC2,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnDC2,Resource.Drawable.buttonyellow);
				SetBackgroundResourceAndKeepPadding(btnGND2,Resource.Drawable.button);
				break;
			case InputCoupling.GND:
				SetBackgroundResourceAndKeepPadding(btnAC2,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnDC2,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnGND2,Resource.Drawable.buttonyellow);
				break;
			default:
				break;
			}
		}

		void UpdateAttenuationFactor2 ()
		{
			switch (Oscilloscope.Channels [1].AttenuationFactor) {
			case AttenuationFactor.X1:
				SetBackgroundResourceAndKeepPadding(btnProbe2,Resource.Drawable.button);
				break;
			case AttenuationFactor.X10:
				SetBackgroundResourceAndKeepPadding(btnProbe2,Resource.Drawable.buttonyellow);
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
				SetBackgroundResourceAndKeepPadding(btnTriggerCH1,Resource.Drawable.buttoncyan);
				SetBackgroundResourceAndKeepPadding(btnTriggerCH2,Resource.Drawable.button);
			} else {
				SetBackgroundResourceAndKeepPadding(btnTriggerCH1,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnTriggerCH2,Resource.Drawable.buttoncyan);
			}
		}

		void UpdateTriggerSlopeUI ()
		{
			if (Oscilloscope.Trigger.Slope == TriggerSlope.Rising) {
				SetBackgroundResourceAndKeepPadding(btnSlopeUp,Resource.Drawable.slopeupselected);
				SetBackgroundResourceAndKeepPadding(btnSlopeDown,Resource.Drawable.slopedownunselected);
			} else {
				SetBackgroundResourceAndKeepPadding(btnSlopeUp,Resource.Drawable.slopeupunselected);
				SetBackgroundResourceAndKeepPadding(btnSlopeDown,Resource.Drawable.slopedownselected);
			}
		}

		void UpdateTriggerModeUI ()
		{
			switch (Oscilloscope.Trigger.Mode) {
			case TriggerMode.Normal:
				SetBackgroundResourceAndKeepPadding(btnNrml,Resource.Drawable.buttoncyan);
				SetBackgroundResourceAndKeepPadding(btnOnce,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnRun,Resource.Drawable.button);
				break;
			case TriggerMode.Once:
				SetBackgroundResourceAndKeepPadding(btnNrml,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnOnce,Resource.Drawable.buttoncyan);
				SetBackgroundResourceAndKeepPadding(btnRun,Resource.Drawable.button);
				break;
			case TriggerMode.Run:
				SetBackgroundResourceAndKeepPadding(btnNrml,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnOnce,Resource.Drawable.button);
				SetBackgroundResourceAndKeepPadding(btnRun,Resource.Drawable.buttoncyan);
				break;
			default:
				break;
			}
		}

		void UpdateHoldUI ()
		{
			if (Oscilloscope.Hold)
				SetBackgroundResourceAndKeepPadding(btnHold,Resource.Drawable.buttoncyan);
			else
				SetBackgroundResourceAndKeepPadding(btnHold,Resource.Drawable.button);
		}

		void UpdateAutorangeUI ()
		{
			if (Oscilloscope.AutoRange)
				SetBackgroundResourceAndKeepPadding(btnAutoRange,Resource.Drawable.buttonred);
			else
				SetBackgroundResourceAndKeepPadding(btnAutoRange,Resource.Drawable.button);
		}

		void UpdateTimeBaseText ()
		{
			lblTimebase.Text = TimeBaseConverter.ToString (Oscilloscope.TimeBase);
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

			lblVolt1 = FindViewById<TextView> (Resource.Id.txtVDIV1);
			lblVolt2 = FindViewById<TextView> (Resource.Id.txtVDIV2);

			lblTimebase = FindViewById<TextView> (Resource.Id.txtTimebase);
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
			Service.Execute (new PreviousTimeBaseCommand ());
		}

		void HandleTimebaseLeftClick (object sender, EventArgs e)
		{
			Service.Execute (new NextTimeBaseCommand ());
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
			//_ScopeLayout.SelectedChannel = 1;
			UpdateScopeControls ();
		}

		void HandleCH1Click (object sender, EventArgs e)
		{
			_ScopeView.SelectedChannel = 0;
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

		private void SetBackgroundResourceAndKeepPadding(View view,int resource)
		{
			int top = view.PaddingTop;
			int left = view.PaddingLeft;
			int right = view.PaddingRight;
			int bottom = view.PaddingBottom;

			view.SetBackgroundResource (resource);
			view.SetPadding(left, top, right, bottom);
		}

	}
}


