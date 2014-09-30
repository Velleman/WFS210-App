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
		UIButton btnCalibrate { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblDemo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblMarkers { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch swDemo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch swMarker { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtWifiName { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnCalibrate != null) {
				btnCalibrate.Dispose ();
				btnCalibrate = null;
			}
			if (lblDemo != null) {
				lblDemo.Dispose ();
				lblDemo = null;
			}
			if (lblMarkers != null) {
				lblMarkers.Dispose ();
				lblMarkers = null;
			}
			if (lblName != null) {
				lblName.Dispose ();
				lblName = null;
			}
			if (swDemo != null) {
				swDemo.Dispose ();
				swDemo = null;
			}
			if (swMarker != null) {
				swMarker.Dispose ();
				swMarker = null;
			}
			if (txtWifiName != null) {
				txtWifiName.Dispose ();
				txtWifiName = null;
			}
		}
	}
}
