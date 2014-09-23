using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace WFS210.UI
{
	partial class SettingsViewController : UIViewController
	{
		public WifiSettings WifiSetting{ get; set;}
		public SettingsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			btnDismiss.TouchUpInside += (object sender, EventArgs e) => {
				var parent = this.PresentingViewController as iWFS210ViewController;
				parent.DismissSettingsViewController();
			};

			txtWifiName.Text = WifiSetting.Name;
			txtWifiChannel.Text = WifiSetting.Channel.ToString();

		}


	}
}
