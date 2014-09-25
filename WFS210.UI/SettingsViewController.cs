using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using WFS210.Services;

namespace WFS210.UI
{
	partial class SettingsViewController : UIViewController
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
			btnDismiss.TouchUpInside += (object sender, EventArgs e) => {
				WifiSetting.Channel = int.Parse (txtWifiChannel.Text);
				WifiSetting.Name = txtWifiName.Text;
				WifiSetting.Password = txtWifiPassword.Text;
				if (RequestedDismiss != null)
					RequestedDismiss (this, null);
			};

			swDemo.ValueChanged += (object sender, EventArgs e) => {
				if (swDemo.On) {
					NSUserDefaults.StandardUserDefaults.SetBool (true, "demo");
					ServiceManager.ServiceType = ServiceType.Demo;
				} else {
					NSUserDefaults.StandardUserDefaults.SetBool (false, "demo");
					ServiceManager.ServiceType = ServiceType.Live;
					if (!ServiceManager.ActiveService.Active)
						ServiceManager.ActiveService.Activate ();
					ServiceManager.ActiveService.RequestSettings ();
					ServiceManager.ActiveService.RequestSamples ();
				}
			};

			txtWifiName.Text = WifiSetting.Name;
			txtWifiChannel.Text = WifiSetting.Channel.ToString ();
			if (ServiceManager.ServiceType == ServiceType.Demo)
				swDemo.On = true;
			else
				swDemo.On = false;
		}
	}
}
