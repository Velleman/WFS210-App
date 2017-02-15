
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using WFS210;
using WFS210.Services;
using WFS210.Util;

namespace WFS210.Droid
{
	public class SignalView : View
	{
		public ServiceManager ServiceManager{ get; set; }

		public Oscilloscope Oscilloscope{ get; set; }

		private Rect signalRectangle, largeRectangle;

		public Path[] traces;

		public int SelectedChannel { get; set; }

		public bool IsScaling{ get; private set;}

		public bool ScalingTime{ get; private set;}

		public event EventHandler<NewDataEventArgs> NewData;

		private Paint[] paints;
		private Paint paintGrid;
        private Paint paintScroll;
		private int offSet;

		private float SampleToPointRatio;
		private int TotalSamples;

		private int GrappleDistance;

		private static readonly int InvalidPointerId = -1;

		private readonly ScaleGestureDetector _scaleDetector;

		private int _activePointerId = InvalidPointerId;
		private float _lastTouchX;
		private float _lastTouchY;

		public float horizontalDIVS;

		public Grid Grid{ get; private set;}
		 
		public List<Marker> Markers{ get; private set;}
        public bool DrawMarkers { get; set; }
		Marker _closestMarker;

		private Context _context;

        Stopwatch sw = new Stopwatch();

        Bitmap bmpSignals;
        Canvas cnvsSignals;

		/// <summary>
		/// Gets or sets the service.
		/// </summary>
		/// <value>The service.</value>
		WFS210.Services.Service Service {
			get {
				return ServiceManager.ActiveService;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.Android.SignalView"/> class.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="attrs">Attrs.</param>
		public SignalView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize (context);
			_scaleDetector = new ScaleGestureDetector (context, new MyScaleListener (this));
		}

		/// <summary>
		/// Initialize the SignalView.
		/// </summary>
		/// <param name="context">Context.</param>
		public void Initialize (Context context)
		{
			_context = context;

			//Init paths
			traces = new Path[2];
			traces [0] = new Path ();
			traces [1] = new Path ();

			//init paints
			paints = new Paint[2];
			paints [0] = new Paint ();
			paints [0] = new Paint ();
			paints [0].Color = Color.Green;
			paints [0].StrokeWidth = 2;
			paints [0].SetStyle (Paint.Style.Stroke);

			paints [1] = new Paint ();
			paints [1].Color = Color.Yellow;
			paints [1].StrokeWidth = 2;
			paints [1].SetStyle (Paint.Style.Stroke);

			paintGrid = new Paint ();
			paintGrid.Color = Color.Gray;
			paintGrid.StrokeWidth = 1;
			paintGrid.SetStyle (Paint.Style.Stroke);

            paintGrid = new Paint();
            paintGrid.Color = Color.Gray;
            paintGrid.StrokeWidth = 1;
            paintGrid.SetStyle(Paint.Style.Fill);

			Markers = new List<Marker> ();

			this.SetBackgroundColor (Color.Transparent);

			offSet = 20;

			GrappleDistance = 60;
		}

		/// <param name="e">The motion event.</param>
		/// <summary>
		/// Implement this method to handle touch screen motion events.
		/// </summary>
		/// <returns>To be added.</returns>
		public override bool OnTouchEvent (MotionEvent e)
		{
			_scaleDetector.OnTouchEvent (e);
			MotionEventActions action = e.Action & MotionEventActions.Mask;
			int pointerIndex;
			switch (action) {
			case MotionEventActions.Down:
				_lastTouchX = e.GetX ();
				_lastTouchY = e.GetY ();
				_activePointerId = e.GetPointerId (0);
				_closestMarker = GetMarkerAt (new PointF (e.GetX (), e.GetY ()));
				break;
			case MotionEventActions.Move:
				pointerIndex = e.FindPointerIndex (_activePointerId);
				float x = e.GetX ();
				float y = e.GetY ();
				if (!_scaleDetector.IsInProgress) {
					if (_closestMarker != null) {
						if (_closestMarker is XMarker) {
							var position = _closestMarker.Value;
							if (x > Grid.StartWidth) {
								if (x < Grid.EndWidth)
									position = x;
								else
									position = Grid.EndWidth;
							} else {
								position = Grid.StartWidth;
							}
							_closestMarker.Value = position;
						} else {
							var position = _closestMarker.Value;
							if (y > Grid.StartHeight) {
								if (y < Grid.EndHeight)
									position = y;
								else
									position = Grid.EndHeight;
							} else {
								position = Grid.StartHeight;
							}
							_closestMarker.Value = position;

						}
						Invalidate ();
					}
				}
				_lastTouchX = x;
				_lastTouchY = y;
				break;
			case MotionEventActions.Up:
                if((_closestMarker is ZeroLine || _closestMarker is TriggerMarker) && DrawMarkers)
                    ApplyMarkerValuesToScope();
                break;
			case MotionEventActions.Cancel:
				_closestMarker = null;
				Invalidate ();
				break;
			case MotionEventActions.PointerUp:
				pointerIndex = (int)(e.Action & MotionEventActions.PointerIndexMask) >> (int)MotionEventActions.PointerIndexShift;
				int pointerid = e.GetPointerId (pointerIndex);
				if (pointerid == _activePointerId) {
                    ApplyMarkerValuesToScope();
				}
				break;
			default:
				break;
			}
			return true;
		}

		/// <param name="w">Current width of this view.</param>
		/// <param name="h">Current height of this view.</param>
		/// <param name="oldw">Old width of this view.</param>
		/// <param name="oldh">Old height of this view.</param>
		/// <summary>
		/// This is called during layout when the size of this view has changed.
		/// </summary>
		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			Grid = new Grid (w, h, offSet, offSet);
			signalRectangle = new Rect ((int)Grid.StartWidth, (int)Grid.StartHeight + 1, (int)Grid.EndWidth, (int)Grid.EndHeight);
			largeRectangle = new Rect (0, 0, w, h);
			CalculateRatios ();
			CalculateMarkers ();
            bmpSignals = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);

		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public void Update ()
		{
            if (bmpSignals != null)
            {
                bmpSignals.EraseColor(Color.Transparent);
                cnvsSignals = new Canvas(bmpSignals);
                for (int i = 0; i < 2; i++)
                {
                    traces[i].Rewind();
                    traces[i].MoveTo(Grid.StartWidth, MapSampleDataToScreen(Oscilloscope.Channels[i].Samples[0]));

                    int s = 0;
                    for (int j = (int)Grid.StartWidth; j < MapXPosToScreen(TotalSamples); j++)
                    {
                        traces[i].LineTo(MapXPosToScreen(s) + Grid.StartWidth, MapSampleDataToScreen(Oscilloscope.Channels[i].Samples[s]));
                        s++;
                    }
                }

                if (Service.Oscilloscope.Channels[0].VoltsPerDivision != VoltsPerDivision.VdivNone)
                    cnvsSignals.DrawPath(traces[0], paints[0]);
                if (Service.Oscilloscope.Channels[1].VoltsPerDivision != VoltsPerDivision.VdivNone)
                    cnvsSignals.DrawPath(traces[1], paints[1]);
                Invalidate();
            }
            
		}

		/// <param name="canvas">the canvas on which the background will be drawn</param>
		/// <summary>
		/// Implement this to do your drawing.
		/// </summary>
		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);
            
			Grid.Draw (canvas, paintGrid);

            //Draw Vertical line for scroll container
            canvas.DrawLine(offSet, canvas.Height, offSet, canvas.Height - offSet, paintGrid);
            //Draw Horizontal line for scroll container
            canvas.DrawLine(offSet, canvas.Height-1, canvas.Width, canvas.Height-1, paintGrid);

            canvas.DrawRect(offSet + 2, (canvas.Height - offSet) + 2, offSet + 100, canvas.Height - 2,paintGrid);

			canvas.ClipRect (signalRectangle);

            canvas.DrawBitmap(bmpSignals, 0, 0, null);

            canvas.ClipRect(largeRectangle, Region.Op.Replace);

            if (DrawMarkers)
            {
                foreach (Marker m in Markers)
                {
                    m.Draw(canvas);
                }
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

		private void CalculateMarkers ()
		{
			Markers.Clear ();

			Markers.Add (new ZeroLine (_context, Resource.Drawable.zeroline1, (int)Grid.EndWidth, (int)Grid.StartWidth, (int)(Grid.Height / 2.3)));
			Markers.Add (new ZeroLine (_context, Resource.Drawable.zeroline2, (int)Grid.EndWidth, (int)Grid.StartWidth, (int)(Grid.Height / 1.7)));

			Markers.Add (new YMarker (_context, Resource.Drawable.markera, Grid.EndWidth, (int)(Grid.Height / 3)));
			Markers.Add (new YMarker (_context, Resource.Drawable.markerb, Grid.EndWidth, (int)(Grid.Height * 0.75)));

			Markers.Add (new TriggerMarker (_context, Resource.Drawable.markertriggerup, Resource.Drawable.markertriggerdown, Grid.EndWidth, (int)(Grid.Height / 4)));

			Markers.Add (new XMarker (_context, Resource.Drawable.marker1, (int)Grid.EndHeight, (int)(Grid.Width / 3)));
			Markers.Add (new XMarker (_context, Resource.Drawable.marker2, (int)Grid.EndHeight, (int)(Grid.Width * 0.75)));
		}

        /// <summary>
        /// Applies the marker values to scope object.
        /// </summary>
        private void ApplyMarkerValuesToScope()
        {
            Service.Execute(new YPositionCommand(0, MapScreenDataToScopeData(Markers[0].Value)));
            Service.Execute(new YPositionCommand(1, MapScreenDataToScopeData(Markers[1].Value)));

            var triggerLevel = MapScreenDataToScopeData(Markers[4].Value);
            Service.Execute(new TriggerLevelCommand(triggerLevel));
        }

		/// <summary>
		/// Calculates the ratios.
		/// </summary>
		private void CalculateRatios ()
		{
			SampleToPointRatio = (float)((Grid.EndHeight - Grid.StartHeight) / (Oscilloscope.DeviceContext.UnitsPerDivision * Oscilloscope.DeviceContext.Divisions));
			TotalSamples = (int)(Oscilloscope.DeviceContext.SamplesPerTimeBase * Grid.HorizontalDivs);
		}

		/// <summary>
		/// Maps the sample data to screen.
		/// </summary>
		/// <returns>The sample data to screen.</returns>
		/// <param name="sample">Sample.</param>
		private int MapSampleDataToScreen (int sample)
		{
			var result = (int)((sample * SampleToPointRatio));
			return result + (int)Grid.StartHeight;
		}

		/// <summary>
		/// Maps the screen data to scope data.
		/// </summary>
		/// <returns>The screen data to scope data.</returns>
		/// <param name="value">Value.</param>
		private int MapScreenDataToScopeData (double value)
		{
			return (int)((value - Grid.StartHeight) / SampleToPointRatio);
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
			var ratio = (float)Grid.Width / (float)TotalSamples;
			return (int)(pos * ratio);
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
            
            return DrawMarkers ? closestMarker : null;
		}

		private class MyScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
		{
			private readonly SignalView _view;

			float firstSpanX;
			float firstSpanY;
			int scaleTreshold;
			float spanX,spanY;
			public MyScaleListener (SignalView view)
			{
				_view = view;
				scaleTreshold = 50;
			}

			public override bool OnScale (ScaleGestureDetector detector)
			{
				spanX = detector.CurrentSpanX;
				spanY = detector.CurrentSpanY;

				double angle = (Math.Atan2(spanY,spanX));
				angle = angle * (180.0 / Math.PI);
				if (angle < 45) {
					_view.ScalingTime = true;
					if (firstSpanX >  spanX + scaleTreshold) {
						_view.Service.Execute (new NextTimeBaseCommand ());
						firstSpanX = spanX;
					} else if (firstSpanX < spanX - scaleTreshold){
						_view.Service.Execute (new PreviousTimeBaseCommand ());
						firstSpanX = spanX;

					}
				} else {
					_view.ScalingTime = false;
					if (firstSpanY > spanY + scaleTreshold) {
						firstSpanY = spanY;
						_view.Service.Execute (new PreviousVoltsPerDivisionCommand (_view.SelectedChannel));
					} else if(firstSpanY < spanY - scaleTreshold){
						firstSpanY = spanY;
						_view.Service.Execute (new NextVoltsPerDivisionCommand (_view.SelectedChannel));
					}
				}
				_view.Invalidate ();
				return true;
			}

			public override bool OnScaleBegin (ScaleGestureDetector detector)
			{
				firstSpanX = detector.CurrentSpanX;
				firstSpanY = detector.CurrentSpanY;
				_view.IsScaling = true;
				return base.OnScaleBegin (detector);
			}

			public override void OnScaleEnd (ScaleGestureDetector detector)
			{
				base.OnScaleEnd (detector);
				_view.IsScaling = false;
				//HACK: trigger update on the ui so the volttimeindicator disappears;
				_view.Service.Execute (new TriggerChannelCommand (_view.Oscilloscope.Trigger.Channel));
			}
		}

	}
}

