using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace WFS210.UI
{
	public class PopoverContentViewController<T> : UIViewController
	{
		public UIPickerView pMeasurements;
		T test;
		
		//loads the PopoverContentViewController.xib file and connects it to this object
		public PopoverContentViewController () : base ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var pickerFrame = new RectangleF (0, 0, 200, 350);
			pMeasurements = new UIPickerView (pickerFrame);
			pMeasurements.ShowSelectionIndicator = true;
			pMeasurements.Model = new SignalMeasurementsModel ();
			View.AddSubview (pMeasurements);

		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}

		public class MarkerMeasurementsModel : UIPickerViewModel
		{
			public static string[] markerMeasurements = new string [] {
				"dt",
				"Frequency",
				"dV1",
				"dV2",
				"Enable/Disable Markers",
			};



			public MarkerMeasurementsModel ()
			{

			}

			public override int GetComponentCount (UIPickerView v)
			{
				return 1;
			}

			public override int GetRowsInComponent (UIPickerView pickerView, int component)
			{
				return markerMeasurements.Length;
			}

			public override string GetTitle (UIPickerView picker, int row, int component)
			{
				return markerMeasurements [row];
			}

			public override void Selected (UIPickerView picker, int row, int component)
			{
				var SelectedMeasurement = markerMeasurements [row];
			}

		}

		public class SignalMeasurementsModel : UIPickerViewModel
		{
			public static string[] signalMeasurements = new string [] {
				"Vdc",
				"RMS",
				"TRMS",
				"Vptp",
				"VMax",
				"VMin",
				"W RMS2",
				"W RMS4",
				"W RMS8",
				"W RMS16",
				"W RMS32",
				"Dbm1",
				"Dbm2",
				"DbGain",
			};

			public SignalMeasurementsModel ()
			{

			}

			public override int GetComponentCount (UIPickerView v)
			{
				return 1;
			}

			public override int GetRowsInComponent (UIPickerView pickerView, int component)
			{
				return signalMeasurements.Length;
			}

			public override string GetTitle (UIPickerView picker, int row, int component)
			{
				return signalMeasurements [row];
			}

			public override void Selected (UIPickerView picker, int row, int component)
			{
				var SelectedMeasurement = signalMeasurements [row];
			}
		}

	}
}
