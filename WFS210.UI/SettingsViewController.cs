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

		private UITapGestureRecognizer dismissRecognizer;

		public SettingsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
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

			swMarker.ValueChanged += (object sender, EventArgs e) =>
				NSUserDefaults.StandardUserDefaults.SetBool (swMarker.On, "markers");

			txtWifiName.Text = WifiSetting.SSID;
			swMarker.On = NSUserDefaults.StandardUserDefaults.BoolForKey ("markers");

			if (ServiceManager.ServiceType == ServiceType.Demo)
				swDemo.On = true;
			else
				swDemo.On = false;

			dismissRecognizer = new UITapGestureRecognizer (OnTapOutside);
			dismissRecognizer.NumberOfTapsRequired = 1u;
			dismissRecognizer.Delegate = new DismissGestureRecognizerDelegate (this);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			View.Window.AddGestureRecognizer (dismissRecognizer);
		}

		/// <summary>
		/// If the tap is outside the current view controller then dismiss it
		/// </summary>
		/// <param name="recogniser">Recogniser.</param>
		private void OnTapOutside (UITapGestureRecognizer recogniser)
		{
			if (recogniser.State == UIGestureRecognizerState.Ended) {
				UIView windowView = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
				PointF location = recogniser.LocationInView (null);
				var version = new Version (MonoTouch.Constants.Version);
				if (version > new Version (8,0)) 
				{
					location = new PointF (location.Y, location.X);
				}
				if (!View.PointInside (View.ConvertPointFromView(location,View.Window), null)) {
					var window = View.Window;
					WifiSetting.SSID = txtWifiName.Text;
					if (RequestedDismiss != null) {
						RequestedDismiss (this, new EventArgs ());
					}
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
