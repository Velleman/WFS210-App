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
	[Register ("MeasurementsViewController")]
	partial class MeasurementsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnSubmit { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblMeasurements { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView pMeasurement { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}
			if (lblMeasurements != null) {
				lblMeasurements.Dispose ();
				lblMeasurements = null;
			}
			if (pMeasurement != null) {
				pMeasurement.Dispose ();
				pMeasurement = null;
			}
		}
	}
}
