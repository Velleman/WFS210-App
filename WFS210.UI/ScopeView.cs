using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreAnimation;
using WFS210;
using System.Collections.Generic;

namespace WFS210.UI
{
	public partial class ScopeView : UIView
	{
		public int GrappleDistance { get; set; }

		CGPath path;
		PointF initialPoint;
		PointF latestPoint;
		Oscilloscope wfs210;
		WFS210Tools tool;
		PointF[] scopePoints;
		Trace[] traces = new Trace[2];
		float sampleToPointRatio;
		public Channel SelectedChannel{ get;set;}
		public bool MarkersAreVisible { get; private set;}
		public XMarker[] xMarkers = new XMarker[2];
		public YMarker[] yMarkers = new YMarker[2];
		ZeroLine[] zeroLines = new ZeroLine[2];
		TriggerMarker trigMarker;

		public List<Marker> Markers = new List<Marker> ();

		CALayer gridLayer;

		UIPinchGestureRecognizer pinchGesture;
		UILongPressGestureRecognizer longPressGesture;
		UIPanGestureRecognizer panGesture;
		public VoltTimeIndicator vti;

		public event EventHandler<NewDataEventArgs> NewData;
		/// <summary>
		/// Initializes a new instance of the <see cref="iWFS210.ScopeView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ScopeView (IntPtr handle) : base (handle)
		{
			this.GrappleDistance = 60;

			path = new CGPath ();

			tool = new WFS210Tools ();

			AddGrid ();

			AddXMarkers ();

			AddYMarkers ();

			AddZeroLines ();

			AddTriggerMarker ();

			FillList ();

			RegisterLongPressRecognizer ();

			AddVoltTimeIndicator ();

			RegisterPinchRecognizer ();

		}

		protected virtual void OnNewData(NewDataEventArgs e)
		{
			if (NewData != null)
				NewData (this, e);
		}

		public Oscilloscope Wfs210 {
			set {

				wfs210 = value;
				sampleToPointRatio = (float)this.Bounds.Height / (wfs210.DeviceContext.SampleMax - wfs210.DeviceContext.SampleMin);
			}	
		}

		/// <summary>
		/// Updates the scope view.
		/// </summary>
		public void UpdateScopeView ()
		{
			SampleBuffer buffer = wfs210.Channels [0].Samples;
			scopePoints = new PointF[buffer.Count];
			for (int x = 0; x < buffer.Count; x++) {
				scopePoints [x] = new PointF (x, MapSampleDataToScreen (buffer [x]));
			}
			path.AddLines (scopePoints);
			initialPoint.X = 0;
			initialPoint.Y = buffer [0];
			latestPoint.X = this.Bounds.Width - 1;
			latestPoint.Y = buffer [(int)this.Bounds.Width - 1];
			path.AddLineToPoint (latestPoint);
		}

		private int MapSampleDataToScreen (int sample)
		{
			return (int)(sample * sampleToPointRatio);
		}

		/// <summary>
		/// Called when the user touched the screen
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);

			UITouch touch = touches.AnyObject as UITouch;

			if (touch != null) {
				initialPoint = touch.LocationInView (this);
			}
		}

		/// <summary>
		/// Called when the user drags his finger
		/// </summary>
		/// <param name="touches">Touches.</param>
		/// <param name="evt">Evt.</param>
		public override void TouchesMoved (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			UITouch touch = touches.AnyObject as UITouch;

			if (touch != null) {
			}
		}

		public override void TouchesEnded (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
		}

		/// <summary>
		/// Draws on the specified rect.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public override void Draw (RectangleF rect)
		{
			base.Draw (rect);

			//get graphics context
			using (CGContext g = UIGraphics.GetCurrentContext ()) {

				//set up drawing attributes
				g.SetLineWidth (1);
				UIColor.Red.SetStroke ();

				//add geometry to graphics context and draw it
				g.AddPath (path);		
				g.DrawPath (CGPathDrawingMode.Stroke);
			}       
		}

		public void DisableMarkers ()
		{
			foreach (Marker m in Markers) {
				m.Layer.Opacity = 0f;
			}
			MarkersAreVisible = false;
		}

		public void EnableMarkers ()
		{
			foreach (Marker m in Markers) {
				m.Layer.Opacity = 255f;
			}
			MarkersAreVisible = true;
		}


		private void AddGrid ()
		{
			gridLayer = new CALayer ();
			gridLayer.Bounds = new RectangleF (0, 0, Bounds.Width, Bounds.Height);
			gridLayer.Position = new PointF (Bounds.Width / 2, Bounds.Height / 2);
			gridLayer.Contents = UIImage.FromFile ("VIEWPORT/VIEWPORT-130x78.png").CGImage;
			Layer.AddSublayer (gridLayer);
		}

		private void AddXMarkers ()
		{
			//Makeing XMarkers and adding it to the layers
			xMarkers [0] = new XMarker ("MARKERS/MARKER 1 SLIDER-__x60.png", Convert.ToInt32 (Bounds.Height / 1.5), "XMARKER1");
			xMarkers [1] = new XMarker ("MARKERS/MARKER 2 SLIDER-__x60.png", (int)Bounds.Height / 2, "XMARKER2");
			Layer.AddSublayer (xMarkers [0].Layer);
			Layer.AddSublayer (xMarkers [1].Layer);
		}

		private void AddYMarkers ()
		{
			//Makeing YMarkers and adding it to the layers
			yMarkers [0] = new YMarker ("MARKERS/MARKER A SLIDER-112x__.png", Convert.ToInt32 (Bounds.Width / 1.5), "YMARKER1");
			yMarkers [1] = new YMarker ("MARKERS/MARKER B SLIDER-112x__.png", (int)Bounds.Width / 2, "YMARKER2");
			Layer.AddSublayer (yMarkers [0].Layer);
			Layer.AddSublayer (yMarkers [1].Layer);
		}

		void AddZeroLines ()
		{
			//Makeing XMarkers and adding it to the layers
			zeroLines [0] = new ZeroLine ("ZEROLINE/ZERO-CHAN1-131x__.png", Convert.ToInt32 (Bounds.Height / 5), "ZEROLINE1");
			zeroLines [1] = new ZeroLine ("ZEROLINE/ZERO-CHAN2-131x__.png", (int)Bounds.Height / 6, "ZEROLINE2");
			Layer.AddSublayer (zeroLines [0].Layer);
			Layer.AddSublayer (zeroLines [1].Layer);
		}

		private void AddTriggerMarker ()
		{
			//Makeing TriggerMarkers and adding it to the layers
			trigMarker = new TriggerMarker ("TRIGGER LEVEL/TRIG SLIDER-SLOPE UP-112x__.png", Convert.ToInt32 (Bounds.Height / 3), "TRIGGERMARKER");
			Layer.AddSublayer (trigMarker.Layer);
		}

		void AddVoltTimeIndicator ()
		{
			vti = new VoltTimeIndicator ();

			vti.Hidden = true;

			Layer.AddSublayer (vti.Layer);
		}

		void FillList ()
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
						startDistance = CalculateDistance(firstPoint,secondPoint);
					}
					vti.Hidden = false;
				} else if (pg.State == UIGestureRecognizerState.Changed) {
					float distance;
					if (pg.NumberOfTouches == 2) {
						PointF firstPoint = pg.LocationOfTouch (0, this);
						PointF secondPoint = pg.LocationOfTouch (1, this);
						distance = CalculateDistance(firstPoint,secondPoint);
						Console.WriteLine(distance.ToString());
						if (isHorizontal (firstPoint, secondPoint)) {
							if(distance > startDistance + 50)
							{
								startDistance = distance;
								if (wfs210.TimeBase != TimeBase.Tdiv1s)
									wfs210.TimeBase = wfs210.TimeBase + 1;
							}
							else if(distance < startDistance - 50)
							{
								startDistance = distance;
								if (wfs210.TimeBase != TimeBase.Tdiv1us)
									wfs210.TimeBase = wfs210.TimeBase - 1;
							}
							vti.Text = tool.GetTextFromTimebase(wfs210.TimeBase);
						} else {
							if(distance > startDistance + 50)
							{
								startDistance = distance;
								if (SelectedChannel.VoltsPerDivision != VoltsPerDivision.Vdiv5mV)
									SelectedChannel.VoltsPerDivision = SelectedChannel.VoltsPerDivision + 1;
							}
							else if(distance < startDistance - 50)
							{
								startDistance = distance;
								if (SelectedChannel.VoltsPerDivision != VoltsPerDivision.VdivNone)
									SelectedChannel.VoltsPerDivision = SelectedChannel.VoltsPerDivision - 1;
							}
							vti.Text = tool.GetTextFromVoltPerDivision(SelectedChannel.VoltsPerDivision);
						}
					}
				} else if (pg.State == UIGestureRecognizerState.Ended) {
					vti.Hidden = true;
					OnNewData(new NewDataEventArgs());
				}
			});
			this.AddGestureRecognizer (pinchGesture);
		}

		void RegisterPanGestureRecognizer()
		{
			panGesture = new UIPanGestureRecognizer ((pg) => {
				if (pg.State == UIGestureRecognizerState.Began) {
					Console.WriteLine("PAN BEGAN");
				} else if (pg.State == UIGestureRecognizerState.Changed) {
					Console.WriteLine("PAN CHANGE");
				} else if (pg.State == UIGestureRecognizerState.Ended) {
					Console.WriteLine("PAN END");
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

		private float CalculateDistance(PointF firstPoint, PointF secondPoint)
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
							var position = closestMarker.Position;
							var touchPos = lp.LocationInView(this).X;
							if(touchPos > 0)
							{
								if(touchPos < this.Bounds.Width)
									position.X = lp.LocationInView (this).X;
							}
							closestMarker.Position = position;
						} else {
							var position = closestMarker.Position;
							var touchPos = lp.LocationInView(this).Y;
							if(touchPos > 0)
							{
								if(touchPos < this.Bounds.Height)
									position.Y = lp.LocationInView (this).Y;
							}
							closestMarker.Position = position;
						}
					}
				} else if (lp.State == UIGestureRecognizerState.Ended) {
					closestMarker = null;
					OnNewData(new NewDataEventArgs());
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
				distance = (int)Math.Abs (marker.Position.Y - pt.Y); // vertical distance
			} else {
				distance = (int)Math.Abs (marker.Position.X - pt.X); // horizontal distance
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
