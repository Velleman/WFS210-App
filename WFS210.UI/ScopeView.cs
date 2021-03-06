using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using WFS210;
using WFS210.Util;
using System.Collections.Generic;
using Foundation;
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
		CGPoint initialPoint;
		Oscilloscope wfs210;

		CGPoint[] scopePoints;
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
		CALayer scroll;
		CAShapeLayer scrollBar;
		UIPinchGestureRecognizer pinchGesture;
		UILongPressGestureRecognizer longPressGesture;
		UIPanGestureRecognizer panGesture;
		ServiceManager _ServiceManager;

		public ServiceManager ServiceManager {
			get{ return this._ServiceManager; }
			set {
				this._ServiceManager = value;
				wfs210 = _ServiceManager.ActiveService.Oscilloscope;
				SampleToPointRatio = (float)ScopeBounds.Height / (wfs210.DeviceContext.UnitsPerDivision * wfs210.DeviceContext.Divisions);
				TotalSamples = wfs210.DeviceContext.SamplesPerTimeBase * 15;
			}
		}

		public event EventHandler<NewDataEventArgs> NewData;

		/// <summary>
		/// Initializes a new instance of the <see cref="iWFS210.ScopeView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ScopeView (IntPtr handle) : base (handle)
		{
			this.GrappleDistance = 60;
			Padding = new Padding (17, 0, 18, 0);
			MarkersAreVisible = true;
			LoadGrid ();
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
			LoadXMarkers ();
			LoadYMarkers ();
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
		public CGRect ScopeBounds {
			get { 
				return new CGRect (
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
		Service Service {
			get {
				return ServiceManager.ActiveService;
			}
		}

		int Latestpoint =0;

		/// <summary>
		/// Updates the scope view.
		/// </summary>
		public void Update ()
		{
			for (int i = 0; i < wfs210.Channels.Count; i++) {
				path [i] = new CGPath ();

				SampleBuffer buffer = wfs210.Channels [i].Samples;

				if (buffer.LatestPoint > TotalSamples)
					Latestpoint = TotalSamples;
				else
					Latestpoint = buffer.LatestPoint;

				scopePoints = new CGPoint[Latestpoint];
				var offset = ScrollPosition;
				if (wfs210.Channels [i].VoltsPerDivision != VoltsPerDivision.VdivNone) {
					for (int j = offset; j < offset + scopePoints.Length; j++) {
						scopePoints [j - offset] = new CGPoint (MapXPosToScreen (j - offset) + ScopeBounds.Left, MapSampleDataToScreen (buffer [j]));
					}
				} else {
					for (int j = offset; j < offset + scopePoints.Length; j++) {
						scopePoints [j - offset] = new CGPoint (MapXPosToScreen (j - offset) + ScopeBounds.Left,0);
					}
				}
				path [i].AddLines (scopePoints);
				signals [i].Path = path [i];
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
			gridLayer.Position = new CGPoint ((ScopeBounds.Width / 2) + ScopeBounds.Left, ScopeBounds.Height / 2 + ScopeBounds.Top);
			var image = UIImage.FromBundle ("VIEWPORT/VIEWPORT-130x78");
			gridLayer.Contents = image.CGImage;
			gridLayer.Bounds = new CGRect (0, 0, image.CGImage.Width / image.CurrentScale, image.CGImage.Height / image.CurrentScale);
			Layer.AddSublayer (gridLayer);
		}

		/// <summary>
		/// Gets the signal mask.
		/// </summary>
		/// <returns>The signal mask.</returns>
		CAShapeLayer GetSignalMask ()
		{
			CGRect clippingRect = new CGRect (ScopeBounds.Left, ScopeBounds.Top + 3, ScopeBounds.Width, ScopeBounds.Height - 7);
			// Set clippingRect to the rectangle you wish to clip to

			var maskPath = UIBezierPath.FromRect (clippingRect);
	
			// Create a shape layer
			maskLayer = new CAShapeLayer ();
			maskLayer.Position = new CGPoint (ScopeBounds.Width / 2 + ScopeBounds.Left, ScopeBounds.Height / 2 + ScopeBounds.Top);
			maskLayer.Bounds = new CGRect (ScopeBounds.X, ScopeBounds.Y, ScopeBounds.Width, ScopeBounds.Height);
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
					marker.Position = new CGPoint (rect.X, rect.Y);
					Layer.AddSublayer (marker.Layer);
				}
			}
		}

		/// <summary>
		/// Gets the marker rectangle.
		/// </summary>
		/// <returns>The marker rectangle.</returns>
		/// <param name="marker">Marker.</param>
		CGRect GetMarkerRect (Marker marker)
		{
			if (marker.Layout == MarkerLayout.Vertical) {
				return new CGRect (marker.Value,
					ScopeBounds.Height / 2 + marker.Inlay,
					marker.Image.CGImage.Width,
					ScopeBounds.Height + marker.Inlay);
			} else {
				return new CGRect (ScopeBounds.Width / 2 - marker.Inlay,
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
			XMarkers [0] = new XMarker ("MARKERS/MARKER 1 SLIDER-__x60", Convert.ToInt32 (ScopeBounds.Width / 4), "XMARKER1", 9);
			XMarkers [1] = new XMarker ("MARKERS/MARKER 2 SLIDER-__x60", Convert.ToInt32 (ScopeBounds.Width / 4) * 3, "XMARKER2", 9);
		}

		/// <summary>
		/// Loads the Y markers.
		/// </summary>
		public void LoadYMarkers ()
		{
			//Makeing YMarkers and adding it to the layers
			YMarkers [0] = new YMarker ("MARKERS/MARKER A SLIDER-112x__", Convert.ToInt32 (ScopeBounds.Height / 4), "YMARKER1", -9);
			YMarkers [1] = new YMarker ("MARKERS/MARKER B SLIDER-112x__", Convert.ToInt32 (ScopeBounds.Height / 4) * 3, "YMARKER2", -9);
		}

		/// <summary>
		/// Loads the zero lines.
		/// </summary>
		public void LoadZeroLines ()
		{
			//Makeing ZeroLines and adding it to the layers
			zeroLines [0] = new ZeroLine ("ZEROLINE/ZERO-CHAN1-131x__", MapSampleDataToScreen (wfs210.Channels [0].YPosition), "ZEROLINE1", -17);
			zeroLines [1] = new ZeroLine ("ZEROLINE/ZERO-CHAN2-131x__", MapSampleDataToScreen (wfs210.Channels [0].YPosition), "ZEROLINE2", -17);
		}

		/// <summary>
		/// Loads the trigger marker.
		/// </summary>
		public void LoadTriggerMarker ()
		{
			//Makeing TriggerMarkers and adding it to the layers
			trigMarker = new TriggerMarker ("TRIGGER LEVEL/TRIG SLIDER-SLOPE UP-112x__", MapSampleDataToScreen (wfs210.Trigger.Level), "TRIGGERMARKER", -9);
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

			scroll = new CALayer ();
			var image = UIImage.FromBundle ("VIEWPORT/SCROLL BAR-130x688");
			scroll.Bounds = new CGRect (0, 0, image.CGImage.Width / image.CurrentScale, image.CGImage.Height / image.CurrentScale);
			scroll.Position = new CGPoint (ScopeBounds.Width / 2 + ScopeBounds.Left, ScopeBounds.Height + ScopeBounds.Top);
			scroll.Contents = image.CGImage;
			Layer.AddSublayer (scroll);

			scrollBar = new CAShapeLayer ();
			var path = new CGPath ();
			var data = new CGPoint[2];
			data [0].X = 0 + ScopeBounds.Left + 2;
			data [1].X = 100;
			data [0].Y = ScopeBounds.Height + ScopeBounds.Top;
			data [1].Y = ScopeBounds.Height + ScopeBounds.Top;
			path.AddLines (data);
			scrollBar.LineWidth = 2f;
			scrollBar.StrokeColor = new CGColor (72f / 255f, 72f / 255f, 72f / 255f);
			scrollBar.FillColor = new CGColor (0, 0, 0, 0);
			scrollBar.Path = path;
			Layer.AddSublayer (scrollBar);
		}

		/// <summary>
		/// Updates the scroll indicator.
		/// </summary>
		void UpdateScrollIndicator ()
		{
			var path = new CGPath ();
			var data = new CGPoint[2];
			float ratio = (float)(ScrollPosition / (4096f - (TotalSamples / 2)));
			float scrollRatio = (float)(TotalSamples * ratio);
			data [0].X = (MapXPosToScreen ((int)scrollRatio)) + ScopeBounds.Left + 2;
			data [1].X = data [0].X + 87;
			data [0].Y = ScopeBounds.Height + ScopeBounds.Top;
			data [1].Y = ScopeBounds.Height + ScopeBounds.Top;
			path.AddLines (data);
			scrollBar.Path = path;
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
						CGPoint firstPoint = pg.LocationOfTouch (0, this);
						CGPoint secondPoint = pg.LocationOfTouch (1, this);
						startDistance = CalculateDistance (firstPoint, secondPoint);
					}
					VoltTimeIndicator.Hidden = false;
				} else if (pg.State == UIGestureRecognizerState.Changed) {
					float distance;
					if (pg.NumberOfTouches == 2) {
						CGPoint firstPoint = pg.LocationOfTouch (0, this);
						CGPoint secondPoint = pg.LocationOfTouch (1, this);
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
							VoltTimeIndicator.Text = VoltsPerDivisionConverter.ToString (wfs210.Channels [SelectedChannel].VoltsPerDivision, wfs210.Channels [SelectedChannel].AttenuationFactor);
						}
					}
				} else if (pg.State == UIGestureRecognizerState.Ended) {
					VoltTimeIndicator.Hidden = true;
					ApplyMarkerValuesToScope ();
					OnNewData (null);
					Update ();
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
			panGesture = new UIPanGestureRecognizer (() => {
				switch (panGesture.State) {
				case UIGestureRecognizerState.Began:
					previousX = (float)panGesture.LocationOfTouch (0, this).X;
					break;
				case UIGestureRecognizerState.Changed:
					var touch = panGesture.LocationOfTouch (0, this);
					var copy = ScrollPosition;
					var delta = touch.X - previousX;
					copy -= (int)delta;
					copy = (int)Math.Min (Math.Max (copy, 0), 4096 - TotalSamples);
					previousX = (float)touch.X;
					ScrollPosition = copy;
					UpdateScrollIndicator ();
					Update ();
					break;
				case UIGestureRecognizerState.Ended:
					break;
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
		private static bool PointsAreHorizontal (CGPoint firstPoint, CGPoint secondPoint)
		{
			bool horizontal;
			double angle = Math.Atan2 (secondPoint.Y - firstPoint.Y, secondPoint.X - firstPoint.X);
			double sin = Math.Abs (Math.Sin (angle));
			if (sin < Math.Sin (Math.PI / 4))
				horizontal = true;
			else
				horizontal = false;
			return horizontal;
		}

		/// <summary>
		/// Calculates the distance between 2 points.
		/// </summary>
		/// <returns>The distance.</returns>
		/// <param name="firstPoint">First point.</param>
		/// <param name="secondPoint">Second point.</param>
		private float CalculateDistance (CGPoint firstPoint, CGPoint secondPoint)
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
		public int HitTest (CGPoint pt, Marker marker)
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
		public Marker GetMarkerAt (CGPoint pt)
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
