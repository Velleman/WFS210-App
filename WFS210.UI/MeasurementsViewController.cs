using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Drawing;

namespace WFS210.UI
{
	partial class MeasurementsViewController : UIViewController
	{

		/// <summary>
		/// The selected measurement.
		/// </summary>
		public static string SelectedMeasurement = "test";
		/// <summary>
		/// The selected channel. 0 or 1
		/// </summary>
		public static int SelectedChannel = 0;
		public static bool isMarkerMeasurement = true;

		public UITapGestureRecognizer tapRecognizer;

		public MeasurementsViewController (IntPtr handle) : base (handle)
		{
			tapRecognizer = new UITapGestureRecognizer ((sender) => {
				if (sender.State == UIGestureRecognizerState.Ended) {
					PointF location = sender.LocationInView(null); //Passing nil gives us coordinates in the window

					//Then we convert the tap's location into the local view's coordinate system, and test to see if it's in or outside. If outside, dismiss the view.
					if (!View.PointInside (View.ConvertPointFromView(location,this.View), null)) {
						View.RemoveGestureRecognizer(sender);
						using (var parent = PresentingViewController as iWFS210ViewController) {
							parent.DismissMeasurementsViewController ();
						}
					}
				}
			});
			tapRecognizer.NumberOfTapsRequired = 1;
			tapRecognizer.CancelsTouchesInView = false;
			View.AddGestureRecognizer (tapRecognizer);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			btnSubmit.TouchUpInside += (object sender, EventArgs e) => {
				var parent = this.PresentingViewController as iWFS210ViewController;
				parent.DismissMeasurementsViewController ();
			};

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (isMarkerMeasurement) {
				pMeasurement.Model = new MarkerMeasurementsModel ();
				SelectedMeasurement = MarkerMeasurementsModel.markerMeasurements [0];
			} else {
				pMeasurement.Model = new SignalMeasurementsModel ();
				SelectedMeasurement = SignalMeasurementsModel.signalMeasurements [0];
			}
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
				SelectedMeasurement = markerMeasurements [row];
			}

			public override float GetComponentWidth (UIPickerView picker, int component)
			{
				return 400f;
			}

			public override float GetRowHeight (UIPickerView picker, int component)
			{
				return 40f;
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
				SelectedMeasurement = signalMeasurements [row];
			}

			public override float GetComponentWidth (UIPickerView picker, int component)
			{
				return 400f;
			}

			public override float GetRowHeight (UIPickerView picker, int component)
			{
				return 40f;
			}
		}
	}
}
