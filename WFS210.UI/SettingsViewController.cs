using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using WFS210.Services;
using System.Drawing;
using System.Text.RegularExpressions;

namespace WFS210.UI
{
	partial class SettingsViewController : UIViewController, IUIGestureRecognizerDelegate
	{
		public event EventHandler<EventArgs> RequestedDismiss;

		public WifiSettings WifiSetting{ get; set; }

		public ServiceManager ServiceManager{ get; set; }

		private UITapGestureRecognizer dismissRecognizer;

		public SettingsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			btnApply.TouchUpInside += (object sender, EventArgs e) => {
				//Check demo
				if (swDemo.On) {
					NSUserDefaults.StandardUserDefaults.SetBool (true, "demo");
					ServiceManager.ActiveService.Deactivate();
					ServiceManager.ServiceType = ServiceType.Demo;
				} else {
					NSUserDefaults.StandardUserDefaults.SetBool (false, "demo");
					ServiceManager.ServiceType = ServiceType.Live;
					if (!ServiceManager.ActiveService.Active)
						ServiceManager.ActiveService.Activate ();
					ServiceManager.ActiveService.RequestWifiSettings();
					ServiceManager.ActiveService.RequestSettings ();
					ServiceManager.ActiveService.RequestSamples ();
				}

				//Check markers
				NSUserDefaults.StandardUserDefaults.SetBool (swMarker.On, "markers");

				//Check SSID
				if(txtWifiName.Text != WifiSetting.SSID)
				{
					if (Regex.IsMatch(txtWifiName.Text, @"[a-zA-Z0-9-_-]"))
					{
						WifiSetting.SSID = txtWifiName.Text;
						ServiceManager.ActiveService.SendWifiSettings ();
						new UIAlertView ("New SSID", "Please close this app, then connect to the correct wifi network and then open this app again", null, "OK",null).Show();
						RequestDismiss();
					}
					else
					{
						new UIAlertView ("Invalid SSID", "This SSID is not valid it can contain letters ,numbers,'-','_'", null, "OK",null).Show();
					}

				}
				else
				{
					RequestDismiss();
				}
			};

			btnDismiss.TouchUpInside += (object sender, EventArgs e) => this.DismissViewController (true, null);

			txtWifiName.Text = WifiSetting.SSID;
			swMarker.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("markers");

			if (ServiceManager.ServiceType == ServiceType.Demo)
				swDemo.On = true;
			else
				swDemo.On = false;

			btnCalibrate.TouchUpInside += (object sender, EventArgs e) => {
				ServiceManager.ActiveService.RequestCalibration();
				RequestDismiss();
			};

			lblAppVersion.Text = NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleShortVersionString").ToString ();

			lblWifiVersion.Text = WifiSetting.Version;

			lblWifiChannel.Text = WifiSetting.Channel.ToString ();

			lblScopeVersion.Text = ServiceManager.ActiveService.Oscilloscope.FirmwareVersion;

			dismissRecognizer = new UITapGestureRecognizer (OnTapOutside);
			dismissRecognizer.NumberOfTapsRequired = 1u;
			dismissRecognizer.Delegate = new DismissGestureRecognizerDelegate (this);
		}

		private void RequestDismiss()
		{
			if (RequestedDismiss != null)
				RequestedDismiss (this, EventArgs.Empty);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			View.Window.AddGestureRecognizer (dismissRecognizer);
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();
			this.View.Superview.Bounds = new RectangleF (0, 0, 400, 400);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			View.Window.RemoveGestureRecognizer (dismissRecognizer);
		}

		/// <summary>
		/// If the tap is outside the current view controller then dismiss it
		/// </summary>
		/// <param name="recogniser">Recogniser.</param>
		private void OnTapOutside (UITapGestureRecognizer recogniser)
		{
			if (recogniser.State == UIGestureRecognizerState.Ended) {
				PointF location = recogniser.LocationInView (null);
				var version = new Version(UIDevice.CurrentDevice.SystemVersion);

				if (version > new Version (8,0)) 
				{
					location = new PointF (location.Y, location.X);
				}
				var convertedPoint = View.ConvertPointFromView (location, View.Window);
				if (!View.PointInside (convertedPoint, null)) {
					RequestDismiss();
				}
			}
		}

		public class DismissGestureRecognizerDelegate : UIGestureRecognizerDelegate
		{
			public DismissGestureRecognizerDelegate (SettingsViewController controller)
			{
			}

			public override bool ShouldBegin (UIGestureRecognizer recognizer)
			{
				return true;
			}

			public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
			{
				return true;
			}

			public override bool ShouldRecognizeSimultaneously (UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
			{
				return true;
			}


		}
	}
}
