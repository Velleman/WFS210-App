using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using WFS210.Services;
using System.Drawing;

namespace WFS210.UI
{
	partial class SettingsViewController : UIViewController, IUIGestureRecognizerDelegate
	{
		public event EventHandler<EventArgs> RequestedDismiss;

		public WifiSettings WifiSetting{ get; set; }

		public ServiceManager ServiceManager{ get; set; }

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
					WifiSetting.SSID = txtWifiName.Text;
					ServiceManager.ActiveService.SendWifiSettings ();
					new UIAlertView ("New SSID", "Please close this app, then connect to the correct wifi network and then open this app again", null, "OK",null).Show();
				}

			};

			btnDismiss.TouchUpInside += (object sender, EventArgs e) => {
				if (RequestedDismiss != null) {
					RequestedDismiss (this, new EventArgs ());
				}
			};

			txtWifiName.Text = WifiSetting.SSID;
			swMarker.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("markers");

			if (ServiceManager.ServiceType == ServiceType.Demo)
				swDemo.On = true;
			else
				swDemo.On = false;

			btnCalibrate.TouchUpInside += (object sender, EventArgs e) => {
				ServiceManager.ActiveService.RequestCalibration();
				if(RequestedDismiss != null)
				{
					RequestedDismiss(this,new EventArgs());
				}
			};
		}
	}
}
