// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace WFS210.UI
{
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnDismiss { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblChannel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblDemo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch swDemo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtWifiChannel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtWifiName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtWifiPassword { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnDismiss != null) {
				btnDismiss.Dispose ();
				btnDismiss = null;
			}
			if (lblChannel != null) {
				lblChannel.Dispose ();
				lblChannel = null;
			}
			if (lblDemo != null) {
				lblDemo.Dispose ();
				lblDemo = null;
			}
			if (lblName != null) {
				lblName.Dispose ();
				lblName = null;
			}
			if (lblPassword != null) {
				lblPassword.Dispose ();
				lblPassword = null;
			}
			if (swDemo != null) {
				swDemo.Dispose ();
				swDemo = null;
			}
			if (txtWifiChannel != null) {
				txtWifiChannel.Dispose ();
				txtWifiChannel = null;
			}
			if (txtWifiName != null) {
				txtWifiName.Dispose ();
				txtWifiName = null;
			}
			if (txtWifiPassword != null) {
				txtWifiPassword.Dispose ();
				txtWifiPassword = null;
			}
		}
	}
}
