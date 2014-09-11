using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace WFS210.UI
{
	partial class MeasurementsViewController : UIViewController
	{

		/// <summary>
		/// The selected measurement.
		/// </summary>
		public static string SelectedMeasurement ="test";
		/// <summary>
		/// The selected channel. 0 or 1
		/// </summary>
		public static int SelectedChannel = 0;
		public static bool isMarkerMeasurement = true;
		public MeasurementsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			btnSubmit.TouchUpInside += (object sender, EventArgs e) => {
				var parent = this.PresentingViewController as iWFS210ViewController;
				parent.DismissMeasurementsViewController ();
			};
			if (isMarkerMeasurement)
				pMeasurement.Model = new MarkerMeasurementsModel (this);
			else
				pMeasurement.Model = new SignalMeasurementsModel (this);
		}

		public class MarkerMeasurementsModel : UIPickerViewModel {
			static string [] markerMeasurements = new string [] {
				"dt",
				"Frequency",
				"dV1",
				"dV2",
				"Enable/Disable Markers",
			};

			MeasurementsViewController mvc;
			public MarkerMeasurementsModel (MeasurementsViewController mvc) {
				this.mvc = mvc;
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

		public class SignalMeasurementsModel : UIPickerViewModel {
			static string [] signalMeasurements = new string [] {
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

			MeasurementsViewController mvc;
			public SignalMeasurementsModel (MeasurementsViewController mvc) {
				this.mvc = mvc;
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
