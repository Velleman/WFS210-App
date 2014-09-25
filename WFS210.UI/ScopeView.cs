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

		CGPath[] path;
		PointF initialPoint;
		Service service;
		Oscilloscope wfs210;

		PointF[] scopePoints;
		public float sampleToPointRatio;

		public int SelectedChannel{ get; set; }

		public bool MarkersAreVisible { get; private set; }

		public XMarker[] xMarkers = new XMarker[2];
		public YMarker[] yMarkers = new YMarker[2];
		ZeroLine[] zeroLines = new ZeroLine[2];
		TriggerMarker trigMarker;
		CALayer gridLayer;
		public List<Marker> Markers = new List<Marker> ();
		CAShapeLayer[] signals;
		CAShapeLayer maskLayer;
		public VoltTimeIndicator vti;

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

			RegisterLongPressRecognizer ();

			LoadVoltTimeIndicator ();

			RegisterPinchRecognizer ();
		}

		public void Initialize ()
		{

			LoadZeroLines ();

			LoadTriggerMarker ();

			path = new CGPath[wfs210.Channels.Count];

			signals = new CAShapeLayer[wfs210.Channels.Count];

			LoadSignals ();

			signals [0].StrokeColor = new CGColor (0, 255, 0);
			signals [1].StrokeColor = new CGColor (255, 255, 0);

			FillList ();

		}

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

		protected virtual void OnNewData (NewDataEventArgs e)
		{
			if (NewData != null)
				NewData (this, e);
		}

		public Service Service {
			set {

				this.service = value;
				wfs210 = this.service.Oscilloscope;
				sampleToPointRatio = (float)ScopeBounds.Height / (wfs210.DeviceContext.UnitsPerDivision * wfs210.DeviceContext.Divisions);
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
				path [i] = new CGPath ();
				SampleBuffer buffer = wfs210.Channels [i].Samples;
				scopePoints = new PointF[TotalSamples];
				for (int x = 0; x < TotalSamples; x++) {
					scopePoints [x] = new PointF (MapXPosToScreen (x) + ScopeBounds.Left, MapSampleDataToScreen (buffer [x]));
				}
				path [i].AddLines (scopePoints);
				SetNeedsDisplay ();
			}
		}


		private int MapSampleDataToScreen (int sample)
		{
			var result = (int)((sample * sampleToPointRatio) + ScopeBounds.Top);
			return result;
		}

		private int ScreenDataToScopeData (int value)
		{
			return (int)((value - ScopeBounds.Top) / sampleToPointRatio);
		}

		private int ScreenDataToScopeDataInverted (int value)
		{
			return 255 - (int)(value / sampleToPointRatio);
		}

		private int MapXPosToScreen (int pos)
		{
			var totalSamples = TotalSamples;
			var ratio = ScopeBounds.Width / totalSamples;
			return (int)(pos * ratio);
		}

		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			using (CGContext g = UIGraphics.GetCurrentContext ()) {
				LoadMarkers ();
				for (int i = 0; i < wfs210.Channels.Count; i++) {
					signals [i].Path = path [i];
				}

			}
		}


		void LoadGrid ()
		{
			gridLayer = new CALayer ();
			gridLayer.Bounds = new RectangleF (0, 0, ScopeBounds.Width, ScopeBounds.Height);
			gridLayer.Position = new PointF (ScopeBounds.Width / 2 + ScopeBounds.Left, ScopeBounds.Height / 2 + ScopeBounds.Top);
			gridLayer.Contents = UIImage.FromFile ("VIEWPORT/VIEWPORT-130x78.png").CGImage;
			Layer.AddSublayer (gridLayer);
		}

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

		public void ToggleMarkers ()
		{
			MarkersAreVisible = !MarkersAreVisible;
		}


		public void LoadXMarkers ()
		{
			//Makeing XMarkers and adding it to the layers
			xMarkers [0] = new XMarker ("MARKERS/MARKER 1 SLIDER-__x60.png", Convert.ToInt32 (ScopeBounds.Width / 4), "XMARKER1", 9);
			xMarkers [1] = new XMarker ("MARKERS/MARKER 2 SLIDER-__x60.png", Convert.ToInt32 (ScopeBounds.Width / 4) * 3, "XMARKER2", 9);
		}

		public void LoadYMarkers ()
		{
			//Makeing YMarkers and adding it to the layers
			yMarkers [0] = new YMarker ("MARKERS/MARKER A SLIDER-112x__.png", Convert.ToInt32 (ScopeBounds.Height / 4), "YMARKER1", -9);
			yMarkers [1] = new YMarker ("MARKERS/MARKER B SLIDER-112x__.png", Convert.ToInt32 (ScopeBounds.Height / 4) * 3, "YMARKER2", -9);
		}

		public void LoadZeroLines ()
		{
			//Makeing ZeroLines and adding it to the layers
			zeroLines [0] = new ZeroLine ("ZEROLINE/ZERO-CHAN1-131x__.png", MapSampleDataToScreen (wfs210.Channels [0].YPosition), "ZEROLINE1", -18);
			zeroLines [1] = new ZeroLine ("ZEROLINE/ZERO-CHAN2-131x__.png", MapSampleDataToScreen (wfs210.Channels [0].YPosition), "ZEROLINE2", -18);
		}

		public void LoadTriggerMarker ()
		{
			//Makeing TriggerMarkers and adding it to the layers
			trigMarker = new TriggerMarker ("TRIGGER LEVEL/TRIG SLIDER-SLOPE UP-112x__.png", MapSampleDataToScreen (wfs210.Trigger.Level), "TRIGGERMARKER", -9);
		}

		public void LoadVoltTimeIndicator ()
		{
			vti = new VoltTimeIndicator ();

			vti.Hidden = true;

			vti.Layer.ZPosition = 100;

			Layer.AddSublayer (vti.Layer);
		}

		public void FillList ()
		{
			Markers.Add (xMarkers [0]);
			Markers.Add (xMarkers [1]);
			Markers.Add (yMarkers [0]);
			Markers.Add (yMarkers [1]);
			Markers.Add (trigMarker);
			Markers.Add (zeroLines [0]);
			Markers.Add (zeroLines [1]);
		}



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
					vti.Hidden = false;
				} else if (pg.State == UIGestureRecognizerState.Changed) {
					float distance;
					if (pg.NumberOfTouches == 2) {
						PointF firstPoint = pg.LocationOfTouch (0, this);
						PointF secondPoint = pg.LocationOfTouch (1, this);
						distance = CalculateDistance (firstPoint, secondPoint);
						if (isHorizontal (firstPoint, secondPoint)) {
							if (distance > startDistance + 50) {
								startDistance = distance;
								wfs210.TimeBase = wfs210.TimeBase.Cycle (-1);
							} else if (distance < startDistance - 50) {
								startDistance = distance;
								wfs210.TimeBase = wfs210.TimeBase.Cycle (1);
							}
							vti.Text = TimeBaseConverter.ToString (wfs210.TimeBase);
						} else {
							if (distance > startDistance + 50) {
								startDistance = distance;
								Service.Execute (new NextVoltsPerDivisionCommand (SelectedChannel));
							} else if (distance < startDistance - 50) {
								startDistance = distance;
								Service.Execute (new PreviousVoltsPerDivisionCommand (SelectedChannel));
							}
							vti.Text = VoltsPerDivisionConverter.ToString (wfs210.Channels [SelectedChannel].VoltsPerDivision);
						}
					}
				} else if (pg.State == UIGestureRecognizerState.Ended) {
					vti.Hidden = true;
					ApplyMarkerValuesToScope ();
					OnNewData (null);
				}
			});
			this.AddGestureRecognizer (pinchGesture);
		}

		private void ApplyMarkerValuesToScope ()
		{
			Service.Execute (new YPositionCommand (0, ScreenDataToScopeData (zeroLines [0].Value)));
			Service.Execute (new YPositionCommand (1, ScreenDataToScopeData (zeroLines [1].Value)));

			var triggerLevel = ScreenDataToScopeDataInverted (trigMarker.Value);
			Service.Execute(new TriggerLevelCommand(triggerLevel));
		}

		void RegisterPanGestureRecognizer ()
		{
			panGesture = new UIPanGestureRecognizer ((pg) => {
				if (pg.State == UIGestureRecognizerState.Began) {
					Console.WriteLine ("PAN BEGAN");
				} else if (pg.State == UIGestureRecognizerState.Changed) {
					Console.WriteLine ("PAN CHANGE");
				} else if (pg.State == UIGestureRecognizerState.Ended) {
					Console.WriteLine ("PAN END");
				}
			});
			this.AddGestureRecognizer (panGesture);
		}

		private bool isHorizontal (PointF firstPoint, PointF secondPoint)
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

		private float CalculateDistance (PointF firstPoint, PointF secondPoint)
		{
			float distance = (float)Math.Sqrt ((firstPoint.X - secondPoint.X) * (firstPoint.X - secondPoint.X) +
			                 (firstPoint.Y - secondPoint.Y) * (firstPoint.Y - secondPoint.Y));
			return distance;
		}

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
