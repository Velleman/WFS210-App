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
using System.Timers;
using Android.Preferences;
using Android.Net.Wifi;
using Android.Net;
using Java.Net;
using Android.Util;

namespace WFS210.Droid
{
	[Activity (Label = "WFS210", MainLauncher = true, Icon = "@drawable/icon",ScreenOrientation = ScreenOrientation.SensorLandscape, Theme="@android:style/Theme.NoTitleBar.Fullscreen")]
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
		/// The calibrating indicator.
		/// </summary>
		private TextView _CalibratingIndicator;

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <value>The service.</value>
		public WFS210.Services.Service Service {
			get { return ServiceManager.ActiveService; }
		}

		private Timer _UpdateTimer;

		/// <summary>
		/// The display settings.
		/// </summary>
		public DisplaySettings DisplaySettings;

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
			btnVoltDown1, btnVoltDown2,
			btnSignalMeasurements,btnSignalMeasurements2,
			btnMarkerMeasurements,btnMarkerMeasurements2,
			btnSettings;
		private TextView lblVolt1,lblVolt2,lblTimebase,txtMeasurement1,txtMarkerMeasurement1,txtMeasurement2,txtMarkerMeasurement2;
		private Spinner spMarkerMeasurement1,spMarkerMeasurement2,spMeasurement1,spMeasurement2;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Create the Oscilloscope
			this.Oscilloscope = new Oscilloscope ();

			//Create the ServiceManager
			this.ServiceManager = new ServiceManager (Oscilloscope, ServiceType.Demo);
            //Set the IPAdress based on the gateway
            var liveService = this.ServiceManager.GetService(ServiceType.Live) as LiveService;
            liveService.Connection.IPAddress = GetIPAdress();

			this._ScopeView = FindViewById<SignalView> (Resource.Id.SignalView);
            _ScopeView.SetLayerType(LayerType.Hardware, null);
			this._VoltTimeIndicator = FindViewById<TextView> (Resource.Id.VoltTimeIndicator);
            //TODO Check API Compatibility 
			//this._VoltTimeIndicator.SetZ (999f);

			this._CalibratingIndicator = FindViewById<TextView> (Resource.Id.CalibratingIndicator);
            //TODO Check API Compatibility
			//this._CalibratingIndicator.SetZ (999f);

			this.DisplaySettings = new DisplaySettings (MarkerUnit.dt, SignalUnit.Vdc);
			LoadControls();
			ServiceManager.SettingsChanged += HandleSettingsChanged;
			ServiceManager.FullFrame += HandleFullFrame;
			this._ScopeView.ServiceManager = this.ServiceManager;
			this._ScopeView.Oscilloscope = this.Oscilloscope;
			_ScopeView.SelectedChannel = 0;
			_ScopeView.NewData += (object sender, NewDataEventArgs e) => RunOnUiThread (() => UpdateMeasurements());
            _ScopeView.DrawMarkers = true;

            
		}

		protected override void OnResume ()
		{
			base.OnResume ();

            //Set the IPAdress based on the gateway
            var liveService = this.ServiceManager.GetService(ServiceType.Live) as LiveService;
            liveService.Connection.IPAddress = GetIPAdress();

			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (this);
			if (prefs.GetBoolean ("DEMO", true)) {
				this.ServiceManager.ServiceType = ServiceType.Demo;
                RunOnUiThread(() => { AlertUser("Demo Mode", "Demo mode is now running"); });
			} else {
				this.ServiceManager.ServiceType = ServiceType.Live;
				if (!Service.Active) {
                    if (!Service.Activate())
                    {
                        this.ServiceManager.ServiceType = ServiceType.Demo;
                        Service.Activate();
                        RunOnUiThread(() => { AlertUser("No Connection", "No WFS210 found\r\nDemo mode is now running"); });
                        var editor = prefs.Edit();
                        editor.PutBoolean("DEMO", true);
                        editor.Commit();
                    }
                    else
                    {
                        Service.RequestWifiSettings();
                        Service.RequestSettings();
                        Service.RequestSamples();
                    }
				}
			}

			if (prefs.GetBoolean ("CALIBRATE", false)) {
				Service.RequestCalibration ();
				var editor = prefs.Edit ();
				editor.PutBoolean ("CALIBRATE",false);
				editor.Apply ();
			}

            _ScopeView.DrawMarkers = prefs.GetBoolean("MARKERS",true);

			_UpdateTimer = new Timer (200);
			_UpdateTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => {
				_UpdateTimer.Stop();
				Service.Update ();
                RunOnUiThread(() => _ScopeView.Update());
				_UpdateTimer.Start();
			};
			_UpdateTimer.Enabled = true;
			_UpdateTimer.Start ();

			UpdateScopeControls ();            
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_UpdateTimer.Close ();
            _UpdateTimer.Dispose();
		}

        private string GetIPAdress()
        {
            var wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);

            return GetStringIP(wifiManager.DhcpInfo.Gateway);
        }

        string GetStringIP(int ip)
        {
            int[] bytes = new int[4];
            bytes[0] = ip & 0xFF;
            bytes[1] = (ip >> 8) & 0xFF;
            bytes[2] = (ip >> 16) & 0xFF;
            bytes[3] = (ip >> 24) & 0xFF;
            var s =  "" + bytes[0] + "." + bytes[1] + "." + bytes[2] + "." + bytes[3];
            return s;
        }

		private void AlertUser(string title,string message)
		{
			// 1. Instantiate an AlertDialog.Builder with its constructor
			AlertDialog.Builder builder = new AlertDialog.Builder(this);

			// 2. Chain together various setter methods to set the dialog characteristics
			builder.SetMessage(message).SetTitle(title);
			builder.SetCancelable(false);
			builder.SetPositiveButton ("OK", (senderAlert, args) => {
				//change value write your own set of instructions
				//you can also create an event for the same in xamarin
				//instead of writing things here
				Console.WriteLine("Dismissed");
			});
			builder.Show ();
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
			if (Service.Oscilloscope.Calibrating) {
				_CalibratingIndicator.Visibility = ViewStates.Visible;	
			} else {
				if(_CalibratingIndicator.Visibility != ViewStates.Invisible)
					_CalibratingIndicator.Visibility = ViewStates.Invisible;
			}
			UpdateChannel1UI ();
			UpdateChannel2UI ();
			UpdateTriggerUI ();
			UpdateMeasurements ();
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
            //Log.Debug("Update Measurements", "timestamp");
            txtMeasurement1.SetLayerType(LayerType.Software, null);
            txtMarkerMeasurement1.SetLayerType(LayerType.Software, null);
            txtMeasurement2.SetLayerType(LayerType.Software, null);
            txtMarkerMeasurement2.SetLayerType(LayerType.Software, null);
            txtMeasurement1.Text = GetMeasurementString(DisplaySettings.SignalUnits[0], 0);
            txtMeasurement2.Text = GetMeasurementString(DisplaySettings.SignalUnits[1], 1);
            txtMarkerMeasurement1.Text = GetMarkerMeasurementString(DisplaySettings.MarkerUnits[0]);
            txtMarkerMeasurement2.Text = GetMarkerMeasurementString(DisplaySettings.MarkerUnits[1]);

            
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

			spMarkerMeasurement1 = FindViewById<Spinner> (Resource.Id.spMarkerMeasurement1);
			var markerMeasurementAdapater1 = ArrayAdapter.CreateFromResource(this,Resource.Array.MarkerMeasurements,Android.Resource.Layout.SimpleSpinnerItem);
			markerMeasurementAdapater1.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spMarkerMeasurement1.Adapter = markerMeasurementAdapater1;
			spMarkerMeasurement1.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_selected);

			//Get spinner and bind Array adapter to it
			spMeasurement1 = FindViewById<Spinner> (Resource.Id.spMeasurement1);
			var measurementAdapater1 = ArrayAdapter.CreateFromResource(this,Resource.Array.Measurements,Android.Resource.Layout.SimpleSpinnerItem);
			measurementAdapater1.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spMeasurement1.Adapter = measurementAdapater1;
			spMeasurement1.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_selected);

			btnSignalMeasurements = FindViewById<Button> (Resource.Id.fakebutton1);
			btnSignalMeasurements.Click += HandleSignalMeasurementsClick;

			btnMarkerMeasurements = FindViewById<Button> (Resource.Id.fakeMarkerButton1);
			btnMarkerMeasurements.Click += HandleMarkerMeasurementsClick;

			txtMeasurement1 = FindViewById<TextView> (Resource.Id.txtMeasurement1);
			txtMarkerMeasurement1 = FindViewById<TextView> (Resource.Id.txtMarkerMeasurement1);


			spMarkerMeasurement2 = FindViewById<Spinner> (Resource.Id.spMarkerMeasurement2);
			var markerMeasurementAdapater2 = ArrayAdapter.CreateFromResource(this,Resource.Array.MarkerMeasurements,Android.Resource.Layout.SimpleSpinnerItem);
			markerMeasurementAdapater2.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spMarkerMeasurement2.Adapter = markerMeasurementAdapater2;
			spMarkerMeasurement2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_selected);

			spMeasurement2 = FindViewById<Spinner> (Resource.Id.spMeasurement2);
			var measurementAdapater2 = ArrayAdapter.CreateFromResource(this,Resource.Array.Measurements,Android.Resource.Layout.SimpleSpinnerItem);
			measurementAdapater2.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spMeasurement2.Adapter = measurementAdapater2;
			spMeasurement2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinner_selected);

			btnSignalMeasurements2 = FindViewById<Button> (Resource.Id.fakebutton2);
			btnSignalMeasurements2.Click += HandleSignalMeasurements2Click;
			btnMarkerMeasurements2 = FindViewById<Button> (Resource.Id.fakeMarkerButton2);
			btnMarkerMeasurements2.Click += HandleMarkerMeasurements2Click;
			txtMeasurement2 = FindViewById<TextView> (Resource.Id.txtMeasurement2);
			txtMarkerMeasurement2 = FindViewById<TextView> (Resource.Id.txtMarkerMeasurement2);

			btnSettings = FindViewById<Button> (Resource.Id.btnSettings);
			btnSettings.Click += (sender, e) => {
				LoadPreferences();
				StartActivity(typeof(PreferenceActivity));
			};
		}

		private void LoadPreferences()
		{
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (this);
			if (this.ServiceManager.ServiceType == ServiceType.Live) {
				var editor = prefs.Edit ();

				editor.PutString ("VERSIONNUMBERSCOPE", ServiceManager.ActiveService.Oscilloscope.FirmwareVersion);
				editor.PutString ("VERSIONNUMBERWIFI", ServiceManager.ActiveService.Oscilloscope.WifiSetting.Version);
				editor.PutString ("WIFINAME", ServiceManager.ActiveService.Oscilloscope.WifiSetting.SSID);
				editor.PutString ("APPVERSION", "1.2");
                editor.PutBoolean("MARKERS", _ScopeView.DrawMarkers);
				editor.Apply ();

			} else if (this.ServiceManager.ServiceType == ServiceType.Demo) {
				var editor = prefs.Edit ();
				editor.PutString ("APPVERSION", "1.2");
                editor.PutBoolean("MARKERS", _ScopeView.DrawMarkers);
				editor.Apply ();
			}
		}

		private void spinner_selected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			if (sender.Equals (spMeasurement1)) {
				DisplaySettings.SignalUnits [0] = (SignalUnit)e.Id;
				return;
			} else if (sender.Equals (spMarkerMeasurement1)) {
                UpdateMarkerUnits(0, e);
                return;
			} else if (sender.Equals (spMeasurement2)) {
				DisplaySettings.SignalUnits [1] = (SignalUnit)e.Id;
				return;
			} else {
                UpdateMarkerUnits(1, e);
				return;
			}
			
		}

        private void UpdateMarkerUnits(int channel, AdapterView.ItemSelectedEventArgs e)
        {
            if (e.Id == 4)
            {
                _ScopeView.DrawMarkers = !_ScopeView.DrawMarkers;
            }
            else
                DisplaySettings.MarkerUnits[channel] = (MarkerUnit)e.Id;
        }

		void HandleMarkerMeasurements2Click (object sender, EventArgs e)
		{
			spMarkerMeasurement2.PerformClick ();
		}

		void HandleSignalMeasurements2Click (object sender, EventArgs e)
		{
			spMeasurement2.PerformClick ();
		}

		void HandleMarkerMeasurementsClick (object sender, EventArgs e)
		{
			spMarkerMeasurement1.PerformClick ();
		}

		void HandleSignalMeasurementsClick (object sender, EventArgs e)
		{
			spMeasurement1.PerformClick ();
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
            _ScopeView.SelectedChannel = 1;
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

		public string GetMarkerMeasurementString (MarkerUnit unit)
		{
			if (_ScopeView.Markers.Count != 0) {
				switch (unit) {
				case MarkerUnit.dt:
					return TimeConverter.ToString (MarkerDataCalculator.CalculateTime (Oscilloscope.TimeBase, _ScopeView.Markers [(int)MarkerID.XMarker1], _ScopeView.Markers [(int)MarkerID.XMarker2], _ScopeView.Grid), 2);
				case MarkerUnit.Frequency:
					return FrequencyConverter.ToString (MarkerDataCalculator.CalculateFrequency (Oscilloscope.TimeBase, _ScopeView.Markers [(int)MarkerID.XMarker1], _ScopeView.Markers [(int)MarkerID.XMarker2], _ScopeView.Grid));
				case MarkerUnit.dV1:
					return Oscilloscope.Channels [0].VoltsPerDivision == VoltsPerDivision.VdivNone ? "--" : VoltageConverter.ToString (MarkerDataCalculator.CalculateDV (Oscilloscope.Channels [0].VoltsPerDivision, Oscilloscope.Channels [0].AttenuationFactor, _ScopeView.Markers [(int)MarkerID.YMarker1], _ScopeView.Markers [(int)MarkerID.YMarker2], _ScopeView.Grid));
				case MarkerUnit.dV2:
					return Oscilloscope.Channels [1].VoltsPerDivision == VoltsPerDivision.VdivNone ? "--" : VoltageConverter.ToString (MarkerDataCalculator.CalculateDV (Oscilloscope.Channels [1].VoltsPerDivision, Oscilloscope.Channels [0].AttenuationFactor, _ScopeView.Markers [(int)MarkerID.YMarker1], _ScopeView.Markers [(int)MarkerID.YMarker2], _ScopeView.Grid));
				default:
					return "?";
				}
			}
			else
				return "?";
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


