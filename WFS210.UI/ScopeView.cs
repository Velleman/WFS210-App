using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreAnimation;
using WFS210;
using WFS210.Util;
using System.Collections.Generic;
using MonoTouch.Foundation;
using WFS210.Services;

namespace WFS210.UI
{
	public partial class ScopeView : UIView
	{
		public Padding Padding { get; set; }

		private int TotalSamples;

		public int GrappleDistance { get; set; }

		public int ScrollPosition { get; set; }

		CGPath[] path;
		PointF initialPoint;
		Service service;
		Oscilloscope wfs210;

		PointF[] scopePoints;
		public float SampleToPointRatio;

		public int SelectedChannel{ get; set; }

		public bool MarkersAreVisible { get; set; }

		public XMarker[] XMarkers = new XMarker[2];
		public YMarker[] YMarkers = new YMarker[2];
		ZeroLine[] zeroLines = new ZeroLine[2];
		TriggerMarker trigMarker;
		CALayer gridLayer;
		public List<Marker> Markers = new List<Marker> ();
		CAShapeLayer[] signals;
		CAShapeLayer maskLayer;
		public VoltTimeIndicator VoltTimeIndicator;
		CalibrationIndicator CalibrationIndicator;
		CAShapeLayer scroll;

		UIPinchGestureRecognizer pinchGesture;
		UILongPressGestureRecognizer longPressGesture;
		UIPanGestureRecognizer panGesture;

		public event EventHandler<NewDataEventArgs> NewData;

		/// <summary>
		/// Initializes a new instance of the <see cref="iWFS210.ScopeView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ScopeView (IntPtr handle) : base (handle)
		{
			this.GrappleDistance = 60;
			Padding = new Padding (18, 0, 18, 0);
			MarkersAreVisible = true;
			LoadGrid ();
			LoadXMarkers ();
			LoadYMarkers ();
			RegisterPanGestureRecognizer ();
			RegisterLongPressRecognizer ();
			LoadVoltTimeIndicator ();
			LoadCalibrationIndicator ();
			RegisterPinchRecognizer ();
			LoadScrollIndicator ();
		}

		/// <summary>
		/// Initialize's scopeview.
		/// </summary>
		public void Initialize ()
		{
			LoadZeroLines ();
			LoadTriggerMarker ();
			path = new CGPath[wfs210.Channels.Count];
			signals = new CAShapeLayer[wfs210.Channels.Count];
			LoadSignals ();
			signals [0].StrokeColor = new CGColor (0, 255, 0);
			signals [1].StrokeColor = new CGColor (255, 255, 0);
			FillMarkersList ();
			LoadMarkers ();
		}

		/// <summary>
		/// Gets the scope bounds.
		/// </summary>
		/// <value>The scope bounds.</value>
		public RectangleF ScopeBounds {
			get { 
				return new RectangleF (
					this.Bounds.X + Padding.Left, 
					this.Bounds.Y + Padding.Top,
					this.Bounds.Width - Padding.Horizontal,
					this.Bounds.Height - Padding.Vertical
				);
			}
		}

		/// <summary>
		/// Raises the new data event.
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void OnNewData (NewDataEventArgs e)
		{
			if (NewData != null)
				NewData (this, e);
		}

		/// <summary>
		/// Gets or sets the service.
		/// </summary>
		/// <value>The service.</value>
		public Service Service {
			set {

				this.service = value;
				wfs210 = this.service.Oscilloscope;
				SampleToPointRatio = (float)ScopeBounds.Height / (wfs210.DeviceContext.UnitsPerDivision * wfs210.DeviceContext.Divisions);
				TotalSamples = wfs210.DeviceContext.SamplesPerTimeBase * 15;
			}	
			get {
				return this.service;
			}
		}

		/// <summary>
		/// Updates the scope view.
		/// </summary>
		public void UpdateScopeView ()
		{
			for (int i = 0; i < wfs210.Channels.Count; i++) {
				if (wfs210.Channels [i].VoltsPerDivision != VoltsPerDivision.VdivNone) {
					path [i] = new CGPath ();

					SampleBuffer buffer = wfs210.Channels [i].Samples;
					scopePoints = new PointF[TotalSamples];
					var offset = 0;
					offset = wfs210.Hold ? ScrollPosition : 0;

					for (int j = offset; j < offset + TotalSamples; j++) {
						scopePoints [j - offset] = new PointF (MapXPosToScreen (j - offset) + ScopeBounds.Left, MapSampleDataToScreen (buffer [j]));
					}
					path [i].AddLines (scopePoints);
					signals [i].Path = path [i];
				} else {
					path [i] = new CGPath ();
					scopePoints = new PointF[TotalSamples];
					for (int j = 0; j < scopePoints.Length; j++) {	
						scopePoints [j] = new PointF (MapXPosToScreen (j) + ScopeBounds.Left, 0);
					}
					path [i].AddLines (scopePoints);
					signals [i].Path = path [i];
				}
			}
			if (MarkersAreVisible) {
				foreach (Marker m in Markers) {
					m.Layer.Hidden = false;
				}
			} else {
				foreach (Marker m in Markers) {
					m.Layer.Hidden = true;
				}
			}
			CalibrationIndicator.Hidden = !wfs210.Calibrating;
		}

		/// <summary>
		/// Maps the sample data to screen.
		/// </summary>
		/// <returns>The sample data to screen.</returns>
		/// <param name="sample">Sample.</param>
		private int MapSampleDataToScreen (int sample)
		{
			var result = (int)((sample * SampleToPointRatio) + ScopeBounds.Top);
			return result;
		}

		/// <summary>
		/// Maps the screen data to scope data.
		/// </summary>
		/// <returns>The screen data to scope data.</returns>
		/// <param name="value">Value.</param>
		private int MapScreenDataToScopeData (int value)
		{
			return (int)((value - ScopeBounds.Top) / SampleToPointRatio);
		}

		/// <summary>
		/// Maps the screen data to scope data inverted.
		/// </summary>
		/// <returns>The screen data to scope data inverted.</returns>
		/// <param name="value">Value.</param>
		private int MapScreenDataToScopeDataInverted (int value)
		{
			return 255 - (int)(value / SampleToPointRatio);
		}

		/// <summary>
		/// Maps the X position to screen.
		/// </summary>
		/// <returns>The X position to screen.</returns>
		/// <param name="pos">Position.</param>
		private int MapXPosToScreen (int pos)
		{
			var totalSamples = TotalSamples;
			var ratio = ScopeBounds.Width / totalSamples;
			return (int)(pos * ratio);
		}

		/// <summary>
		/// Loads the grid.
		/// </summary>
		void LoadGrid ()
		{
			gridLayer = new CALayer ();
			gridLayer.Bounds = new RectangleF (0, 0, ScopeBounds.Width, ScopeBounds.Height);
			gridLayer.Position = new PointF (ScopeBounds.Width / 2 + ScopeBounds.Left, ScopeBounds.Height / 2 + ScopeBounds.Top);
			gridLayer.Contents = UIImage.FromFile ("VIEWPORT/VIEWPORT-130x78.png").CGImage;
			Layer.AddSublayer (gridLayer);
		}

		/// <summary>
		/// Gets the signal mask.
		/// </summary>
		/// <returns>The signal mask.</returns>
		CAShapeLayer GetSignalMask ()
		{
			RectangleF clippingRect = new RectangleF (ScopeBounds.Left, ScopeBounds.Top + 3, ScopeBounds.Width, ScopeBounds.Height - 7);
			// Set clippingRect to the rectangle you wish to clip to

			var maskPath = UIBezierPath.FromRect (clippingRect);
	
			// Create a shape layer
			maskLayer = new CAShapeLayer ();
			maskLayer.Position = new PointF (ScopeBounds.Width / 2 + ScopeBounds.Left, ScopeBounds.Height / 2 + ScopeBounds.Top);
			maskLayer.Bounds = new RectangleF (ScopeBounds.X, ScopeBounds.Y, ScopeBounds.Width, ScopeBounds.Height);
			//maskLayer.BorderColor = new CGColor (255, 0, 0 , 1f);
			// Set the path of the mask layer to be the Bezier path we calculated earlier
			maskLayer.Path = maskPath.CGPath;
			//maskLayer.BorderWidth = 1f;
			//Layer.AddSublayer (maskLayer);
			return maskLayer;
		}

		/// <summary>
		/// Loads the signals.
		/// </summary>
		void LoadSignals ()
		{
			for (int i = 0; i < wfs210.Channels.Count; i++) {
				signals [i] = new CAShapeLayer ();
				signals [i].Path = path [i];
				signals [i].LineWidth = 1f;
				signals [i].StrokeColor = new CGColor (0, 255, 0);
				signals [i].FillColor = new CGColor (0, 0, 0, 0);
				signals [i].Mask = GetSignalMask ();
				Layer.AddSublayer (signals [i]);
			}

		}

		/// <summary>
		/// Loads the markers.
		/// </summary>
		void LoadMarkers ()
		{
			if (MarkersAreVisible) {
				foreach (Marker marker in Markers) {
					var rect = GetMarkerRect (marker);
					marker.Position = new PointF (rect.X, rect.Y);
					Layer.AddSublayer (marker.Layer);
				}
			}
		}

		/// <summary>
		/// Gets the marker rectangle.
		/// </summary>
		/// <returns>The marker rectangle.</returns>
		/// <param name="marker">Marker.</param>
		RectangleF GetMarkerRect (Marker marker)
		{
			if (marker.Layout == MarkerLayout.Vertical) {
				return new RectangleF (marker.Value,
					ScopeBounds.Height / 2 + marker.Inlay,
					marker.Image.CGImage.Width,
					ScopeBounds.Height + marker.Inlay);
			} else {
				return new RectangleF (ScopeBounds.Width / 2 - marker.Inlay,
					marker.Value,
					ScopeBounds.Width + marker.Inlay,
					marker.Image.CGImage.Height);
			}
		}

		/// <summary>
		/// Loads the X markers.
		/// </summary>
		public void LoadXMarkers ()
		{
			//Makeing XMarkers and adding it to the layers
			XMarkers [0] = new XMarker ("MARKERS/MARKER 1 SLIDER-__x60.png", Convert.ToInt32 (ScopeBounds.Width / 4), "XMARKER1", 9);
			XMarkers [1] = new XMarker ("MARKERS/MARKER 2 SLIDER-__x60.png", Convert.ToInt32 (ScopeBounds.Width / 4) * 3, "XMARKER2", 9);
		}

		/// <summary>
		/// Loads the Y markers.
		/// </summary>
		public void LoadYMarkers ()
		{
			//Makeing YMarkers and adding it to the layers
			YMarkers [0] = new YMarker ("MARKERS/MARKER A SLIDER-112x__.png", Convert.ToInt32 (ScopeBounds.Height / 4), "YMARKER1", -9);
			YMarkers [1] = new YMarker ("MARKERS/MARKER B SLIDER-112x__.png", Convert.ToInt32 (ScopeBounds.Height / 4) * 3, "YMARKER2", -9);
		}

		/// <summary>
		/// Loads the zero lines.
		/// </summary>
		public void LoadZeroLines ()
		{
			//Makeing ZeroLines and adding it to the layers
			zeroLines [0] = new ZeroLine ("ZEROLINE/ZERO-CHAN1-131x__.png", MapSampleDataToScreen (wfs210.Channels [0].YPosition), "ZEROLINE1", -18);
			zeroLines [1] = new ZeroLine ("ZEROLINE/ZERO-CHAN2-131x__.png", MapSampleDataToScreen (wfs210.Channels [0].YPosition), "ZEROLINE2", -18);
		}

		/// <summary>
		/// Loads the trigger marker.
		/// </summary>
		public void LoadTriggerMarker ()
		{
			//Makeing TriggerMarkers and adding it to the layers
			trigMarker = new TriggerMarker ("TRIGGER LEVEL/TRIG SLIDER-SLOPE UP-112x__.png", MapSampleDataToScreen (wfs210.Trigger.Level), "TRIGGERMARKER", -9);
		}

		/// <summary>
		/// Loads the volt time indicator.
		/// </summary>
		public void LoadVoltTimeIndicator ()
		{
			VoltTimeIndicator = new VoltTimeIndicator ();

			VoltTimeIndicator.Hidden = true;

			VoltTimeIndicator.Layer.ZPosition = 100;

			Layer.AddSublayer (VoltTimeIndicator.Layer);
		}

		/// <summary>
		/// Loads the calibration indicator.
		/// </summary>
		public void LoadCalibrationIndicator ()
		{
			CalibrationIndicator = new CalibrationIndicator ();

			CalibrationIndicator.Hidden = true;

			CalibrationIndicator.Layer.ZPosition = 100;

			Layer.AddSublayer (CalibrationIndicator.Layer);
		}

		/// <summary>
		/// Loads the scroll indicator.
		/// </summary>
		void LoadScrollIndicator ()
		{
			scroll = new CAShapeLayer ();
			var path = new CGPath ();
			var data = new PointF[2];
			data [0].X = 0 + ScopeBounds.Left;
			data [1].X = 100;
			data [0].Y = ScopeBounds.Height + ScopeBounds.Top - 3;
			data [1].Y = ScopeBounds.Height + ScopeBounds.Top - 3;
			path.AddLines (data);
			scroll.LineWidth = 1f;
			scroll.StrokeColor = new CGColor (62, 64, 64);
			scroll.FillColor = new CGColor (0, 0, 0, 0);
			scroll.Path = path;
			Layer.AddSublayer (scroll);
		}

		/// <summary>
		/// Updates the scroll indicator.
		/// </summary>
		void UpdateScrollIndicator ()
		{
			if (wfs210.Hold) {
				var path = new CGPath ();
				var data = new PointF[2];
				float ratio = (float)(ScrollPosition / (4096f - TotalSamples / 2));
				float scrollRatio = (float)(TotalSamples * ratio);
				data [0].X = (MapXPosToScreen ((int)scrollRatio)) + ScopeBounds.Left;
				data [1].X = data [0].X + 100;
				data [0].Y = ScopeBounds.Height + ScopeBounds.Top - 3;
				data [1].Y = ScopeBounds.Height + ScopeBounds.Top - 3;
				path.AddLines (data);
				scroll.Path = path;
			}
		}

		/// <summary>
		/// Fills the markerslist.
		/// </summary>
		public void FillMarkersList ()
		{
			Markers.Add (XMarkers [0]);
			Markers.Add (XMarkers [1]);
			Markers.Add (YMarkers [0]);
			Markers.Add (YMarkers [1]);
			Markers.Add (trigMarker);
			Markers.Add (zeroLines [0]);
			Markers.Add (zeroLines [1]);
		}

		/// <summary>
		/// Registers the pinch recognizer.
		/// </summary>
		private void RegisterPinchRecognizer ()
		{
			float startDistance;
			startDistance = 0.0f;
			pinchGesture = new UIPinchGestureRecognizer ((pg) => {
				if (pg.State == UIGestureRecognizerState.Began) {
					if (pg.NumberOfTouches == 2) {
						PointF firstPoint = pg.LocationOfTouch (0, this);
						PointF secondPoint = pg.LocationOfTouch (1, this);
						startDistance = CalculateDistance (firstPoint, secondPoint);
					}
					VoltTimeIndicator.Hidden = false;
				} else if (pg.State == UIGestureRecognizerState.Changed) {
					float distance;
					if (pg.NumberOfTouches == 2) {
						PointF firstPoint = pg.LocationOfTouch (0, this);
						PointF secondPoint = pg.LocationOfTouch (1, this);
						distance = CalculateDistance (firstPoint, secondPoint);
						if (PointsAreHorizontal (firstPoint, secondPoint)) {
							if (distance > startDistance + 50) {
								startDistance = distance;
								wfs210.TimeBase = wfs210.TimeBase.Cycle (-1);
							} else if (distance < startDistance - 50) {
								startDistance = distance;
								wfs210.TimeBase = wfs210.TimeBase.Cycle (1);
							}
							VoltTimeIndicator.Text = TimeBaseConverter.ToString (wfs210.TimeBase);
						} else {
							if (distance > startDistance + 50) {
								startDistance = distance;
								Service.Execute (new NextVoltsPerDivisionCommand (SelectedChannel));
							} else if (distance < startDistance - 50) {
								startDistance = distance;
								Service.Execute (new PreviousVoltsPerDivisionCommand (SelectedChannel));
							}
							VoltTimeIndicator.Text = VoltsPerDivisionConverter.ToString (wfs210.Channels [SelectedChannel].VoltsPerDivision);
						}
					}
				} else if (pg.State == UIGestureRecognizerState.Ended) {
					VoltTimeIndicator.Hidden = true;
					ApplyMarkerValuesToScope ();
					OnNewData (null);
					UpdateScopeView ();
				}
			});
			this.AddGestureRecognizer (pinchGesture);
		}

		/// <summary>
		/// Applies the marker values to scope object.
		/// </summary>
		private void ApplyMarkerValuesToScope ()
		{
			Service.Execute (new YPositionCommand (0, MapScreenDataToScopeData (zeroLines [0].Value)));
			Service.Execute (new YPositionCommand (1, MapScreenDataToScopeData (zeroLines [1].Value)));

			var triggerLevel = MapScreenDataToScopeData (trigMarker.Value);
			Service.Execute (new TriggerLevelCommand (triggerLevel));
		}

		/// <summary>
		/// Registers the pan gesture recognizer.
		/// </summary>
		void RegisterPanGestureRecognizer ()
		{
			float previousX = 0;
			panGesture = new UIPanGestureRecognizer ((pg) => {
				if (pg.State == UIGestureRecognizerState.Began) {
					Console.WriteLine ("PAN BEGAN");
					previousX = pg.LocationOfTouch (0, this).X;
				} else if (pg.State == UIGestureRecognizerState.Changed) {

					var touch = pg.LocationOfTouch (0, this);

					var copy = ScrollPosition;
					var delta = touch.X - previousX;
					copy -= (int)delta;
					copy = (int)Math.Min (Math.Max (copy, 0), 4096 - TotalSamples);
					previousX = touch.X;
					if (wfs210.Hold) {
						ScrollPosition = copy;
						UpdateScrollIndicator ();
						UpdateScopeView ();
					}


				} else if (pg.State == UIGestureRecognizerState.Ended) {
					Console.WriteLine ("PAN END");
				}
			});
			panGesture.MaximumNumberOfTouches = 2;
			panGesture.MinimumNumberOfTouches = 2;
			this.AddGestureRecognizer (panGesture);
		}

		/// <summary>
		/// Checks if the Points making a horizontal line.
		/// </summary>
		/// <returns><c>true</c>, if horizontal, <c>false</c> otherwise.</returns>
		/// <param name="firstPoint">First point.</param>
		/// <param name="secondPoint">Second point.</param>
		private static bool PointsAreHorizontal (PointF firstPoint, PointF secondPoint)
		{
			bool horizontal;
			double angle = Math.Atan2 (secondPoint.Y - firstPoint.Y, secondPoint.X - firstPoint.X);
			double sin = Math.Abs (Math.Sin (angle));
			if (sin < Math.Sin (Math.PI / 4))
				horizontal = true;
			else
				horizontal = false;
			//Console.WriteLine (horizontal.ToString ());
			return horizontal;
		}

		/// <summary>
		/// Calculates the distance between 2 points.
		/// </summary>
		/// <returns>The distance.</returns>
		/// <param name="firstPoint">First point.</param>
		/// <param name="secondPoint">Second point.</param>
		private float CalculateDistance (PointF firstPoint, PointF secondPoint)
		{
			float distance = (float)Math.Sqrt ((firstPoint.X - secondPoint.X) * (firstPoint.X - secondPoint.X) +
			                 (firstPoint.Y - secondPoint.Y) * (firstPoint.Y - secondPoint.Y));
			return distance;
		}

		/// <summary>
		/// Registers the long press recognizer.
		/// </summary>
		private void RegisterLongPressRecognizer ()
		{
			Marker closestMarker;
			closestMarker = null;
			longPressGesture = new UILongPressGestureRecognizer ((lp) => {
				if (lp.State == UIGestureRecognizerState.Began) {
					initialPoint = lp.LocationInView (this);
					closestMarker = GetMarkerAt (initialPoint);
				} else if (lp.State == UIGestureRecognizerState.Changed) {
					if (closestMarker != null) {
						if (closestMarker is XMarker) {
							var position = closestMarker.Value;
							var touchPos = lp.LocationInView (this).X;
							if (touchPos > ScopeBounds.Left) {
								if (touchPos < this.Bounds.Width)
									position = (int)lp.LocationInView (this).X;
							}
							closestMarker.Value = position;
						} else {
							var position = closestMarker.Value;
							var touchPos = lp.LocationInView (this).Y;
							if (touchPos > ScopeBounds.Top) {
								if (touchPos < this.Bounds.Height)
									position = (int)lp.LocationInView (this).Y;
							}
							closestMarker.Value = position;
						}
					}
				} else if (lp.State == UIGestureRecognizerState.Ended) {
					closestMarker = null;
					ApplyMarkerValuesToScope ();
					OnNewData (null);
				}
			});
			longPressGesture.MinimumPressDuration = 0.1d;
			longPressGesture.AllowableMovement = 100f;
			this.AddGestureRecognizer (longPressGesture);
		}

		/// <summary>
		/// Returns the distance from the point to the marker. This value can
		/// be used to check if the marker has been hit.
		/// </summary>
		/// <returns>The distance from the point to the marker.</returns>
		/// <param name="pt">Point.</param>
		/// <param name="marker">Marker.</param>
		public int HitTest (PointF pt, Marker marker)
		{
			int distance;

			// Note: If the line is vertical, then we need to return the horizontal distance, and vice versa.

			if (marker.Layout == MarkerLayout.Horizontal) {
				distance = (int)Math.Abs (marker.Value - pt.Y); // vertical distance
			} else {
				distance = (int)Math.Abs (marker.Value - pt.X); // horizontal distance
			}

			return distance;
		}

		/// <summary>
		/// Gets the marker at the specified touch position that is
		/// within grapple distance.
		/// </summary>
		/// <returns>The closest <see cref="WFS210.UI.Marker"/>, null if no marker is within
		/// grapple distance.</returns>
		/// <param name="pt">Touch position.</param>
		public Marker GetMarkerAt (PointF pt)
		{
			Marker closestMarker = null;

			float distance, lastDistance = GrappleDistance;

			// Here we will loop over all markers in an attempt
			// to find the marker closest to the touch position.
			for (int i = 0; i < Markers.Count; i++) {
			
				// First, we calculate the distance between the marker
				// line and the touch position. If this distance is within
				// grapple distance, then we have a hit.
				distance = HitTest (pt, Markers [i]);
				if (distance < GrappleDistance) {

					// Check if this new hit is closer than the previous
					// hit. If it is, the current marker will be the
					// new closest marker.
					if ((i == 0) || (distance < lastDistance)) {

						closestMarker = Markers [i];
						lastDistance = distance; // save
					}
				}
			}

			return closestMarker;
		}

	}
}
