
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using WFS210.Services;
using WFS210;
using Android.Graphics;

namespace WFS210.Android
{
	public class ScopeView : View
	{
		public ServiceManager ServiceManager{ get; set;}

		public Oscilloscope Oscilloscope{ get; set;}

		private ScaleGestureDetector _scaleDetector;
	
		public int GrappleDistance { get; set; }
		private Marker selectedMarker;

		public XMarker[] xMarkers = new XMarker[2];
		public YMarker[] yMarkers = new YMarker[2];
		public ZeroLine[] zeroLines = new ZeroLine[2];
		TriggerMarker trigMarker;
		public List<Marker> markers = new List<Marker> ();
		private Rect newRect;

		public Path[] traces;

		public int SelectedChannel { get; set;}

		public event EventHandler<NewDataEventArgs> NewData;

		private Paint[] paints;
		private Paint paintGrid;

		private int height;
		private int width;

		private float SampleToPointRatio;
		private int TotalSamples;

		private float RasterSpace;

		public float horizontalDIVS;

		private bool MarkersAreVisible;

		private Activity activity;
		/// <summary>
		/// Gets or sets the service.
		/// </summary>
		/// <value>The service.</value>
		WFS210.Services.Service Service {
			get {
				return ServiceManager.ActiveService;
			}
		}

		public ScopeView (Context context) :
			base (context)
		{

		}

		public ScopeView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{

		}

		public ScopeView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{

		}

		public ScopeView(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer) 
		{

		}

		public void Initialize (Activity context)
		{
			activity = context;
			_scaleDetector = new ScaleGestureDetector (context, new ScopeViewScaleListener (this));
			this.GrappleDistance = 60;
			selectedMarker = null;

			//Init paths
			traces = new Path[2];
			traces [0] = new Path ();
			traces [1] = new Path ();

			//init paints
			paints = new Paint[2];
			paints [0] = new Paint ();
			paints[0] = new Paint ();
			paints[0].Color = Color.Green;
			paints[0].StrokeWidth = 2;
			paints[0].SetStyle (Paint.Style.Stroke);

			paints[1] = new Paint ();
			paints[1].Color = Color.Yellow;
			paints[1].StrokeWidth = 2;
			paints[1].SetStyle (Paint.Style.Stroke);

			paintGrid = new Paint ();
			paintGrid.Color = Color.Gray;
			paintGrid.StrokeWidth = 1;
			paintGrid.SetStyle (Paint.Style.Stroke);

			//Markers
			MarkersAreVisible = true;
			this.SetBackgroundColor(Color.Transparent);
			newRect = new Rect (this.ClipBounds);
			newRect.Inset(-20, -20);  //make the rect larger
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			width = w;
			height = h;
			CalculateRatios ();

			LoadZeroLines ();
			LoadTriggerMarker ();
			LoadXMarkers ();
			LoadYMarkers ();
			FillMarkersList ();
		}

		/// <param name="canvas">the canvas on which the background will be drawn</param>
		/// <summary>
		/// Implement this to do your drawing.
		/// </summary>
		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			DrawGrid (canvas);


			//happily draw outside the bound now

			for(int i=0;i < 2;i++)
			{
				traces [i].Reset ();
				traces[i].MoveTo (0, MapSampleDataToScreen (Oscilloscope.Channels [i].Samples [0]));

			for (int j = 0; j < MapXPosToScreen(TotalSamples); j++) {
					traces[i].LineTo(MapXPosToScreen(j), MapSampleDataToScreen (Oscilloscope.Channels [i].Samples [j]));
			}

				canvas.DrawPath (traces[i], paints[i]);
			}

			canvas.ClipRect (newRect, Region.Op.Union);

			if (MarkersAreVisible) {
				foreach (Marker m in markers) {
					m.Draw (canvas);
				}
			}
		}

		public override bool OnTouchEvent (MotionEvent ev)
		{
			_scaleDetector.OnTouchEvent(ev);
			MotionEventActions action = ev.Action & MotionEventActions.Mask;
			int pointerIndex;

			switch (action)
			{
			case MotionEventActions.Down:
				selectedMarker = GetMarkerAt(new PointF(ev.GetX(),ev.GetY()));
				break;

			case MotionEventActions.Move:
				if (selectedMarker != null) {
					if (!_scaleDetector.IsInProgress) {
						if (selectedMarker.Layout == MarkerLayout.Horizontal) {
							selectedMarker.Value  = (int)ev.GetY ();
						} else {
							selectedMarker.Value  = (int)ev.GetX ();
						}
						Invalidate ();
					}
				}
				break;

			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
				selectedMarker = null;
				break;

			case MotionEventActions.PointerUp:
				// We only want to update the last touch position if the the appropriate pointer
				// has been lifted off the screen.
				selectedMarker = null;
				break;
			}
			return true;
		}

		private void DrawGrid(Canvas canvas)
		{
			canvas.DrawLine (0, (float)height / 2f, width, (float)height / 2f, paintGrid);
			for (int i = 0; i < 5; i++) {
				canvas.DrawLine (0, (float)height / 2f + (RasterSpace * i), width, (float)height / 2f + (RasterSpace * i), paintGrid);
				canvas.DrawLine (0, (float)height / 2f - (RasterSpace * i), width, (float)height / 2f - (RasterSpace * i), paintGrid);
			}
			var distance = RasterSpace;
			while (distance < width) {
				canvas.DrawLine (distance, 0, distance, Height, paintGrid);
				distance += RasterSpace;
			}

		}

		/// <summary>
		/// Update the scope data
		/// 
		/// </summary>
		public void Update()
		{
			Invalidate ();
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
		/// Calculates the ratios.
		/// </summary>
		private void CalculateRatios()
		{
			CalculateRaster ();
			SampleToPointRatio = (float)height/(Oscilloscope.DeviceContext.UnitsPerDivision * Oscilloscope.DeviceContext.Divisions);
			TotalSamples = (int)(Oscilloscope.DeviceContext.SamplesPerTimeBase * horizontalDIVS);
		}

		/// <summary>
		/// Calculates the raster.
		/// </summary>
		private void CalculateRaster()
		{
			RasterSpace = height / Oscilloscope.DeviceContext.Divisions;

			var distance = RasterSpace;
			horizontalDIVS = 1;
			while (distance < width) {
				distance += RasterSpace;
				horizontalDIVS++;
			}
			var leftover = width - (distance + RasterSpace);
			horizontalDIVS += (float)(RasterSpace / leftover);
		}

		/// <summary>
		/// Maps the sample data to screen.
		/// </summary>
		/// <returns>The sample data to screen.</returns>
		/// <param name="sample">Sample.</param>
		private int MapSampleDataToScreen (int sample)
		{
			var result = (int)((sample * SampleToPointRatio));
			return result;
		}

		/// <summary>
		/// Maps the screen data to scope data.
		/// </summary>
		/// <returns>The screen data to scope data.</returns>
		/// <param name="value">Value.</param>
		private int MapScreenDataToScopeData (double value)
		{
			return (int)((value) / SampleToPointRatio);
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
			var ratio = (float)width / (float)TotalSamples;
			return (int)(pos * ratio);
		}

		/// <summary>
		/// Loads the X markers.
		/// </summary>
		public void LoadXMarkers ()
		{
			//Makeing XMarkers and adding it to the layers
			xMarkers [0] = new XMarker (activity,Resource.Drawable.marker1,height,width/2 + 30);
			xMarkers [1] = new XMarker (activity,Resource.Drawable.marker1,height,width/2 - 30);
		}

		/// <summary>
		/// Loads the Y markers.
		/// </summary>
		public void LoadYMarkers ()
		{
			//Makeing YMarkers and adding it to the layers
			yMarkers [0] = new YMarker (activity,Resource.Drawable.markera,width,height/2 + 100);
			yMarkers [1] = new YMarker (activity,Resource.Drawable.markerb,width,height/2 - 100);
		}

		/// <summary>
		/// Loads the zero lines.
		/// </summary>
		public void LoadZeroLines ()
		{
			//Makeing ZeroLines and adding it to the layers
			zeroLines [0] = new ZeroLine (activity,Resource.Drawable.zeroline1,width,height/2);
			zeroLines [1] = new ZeroLine (activity,Resource.Drawable.zeroline2,width,height/2);
		}

		/// <summary>
		/// Loads the trigger marker.
		/// </summary>
		public void LoadTriggerMarker ()
		{
			//Makeing TriggerMarkers and adding it to the layers
			trigMarker = new TriggerMarker (activity,Resource.Drawable.markertriggerup,width,height/4);
		}

		/// <summary>
		/// Fills the markerslist.
		/// </summary>
		public void FillMarkersList ()
		{
			markers.Add (xMarkers [0]);
			markers.Add (xMarkers [1]);
			markers.Add (yMarkers [0]);
			markers.Add (yMarkers [1]);
			markers.Add (trigMarker);
			markers.Add (zeroLines [0]);
			markers.Add (zeroLines [1]);
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
			for (int i = 0; i < markers.Count; i++) {

				// First, we calculate the distance between the marker
				// line and the touch position. If this distance is within
				// grapple distance, then we have a hit.
				distance = HitTest (pt, markers [i]);
				if (distance < GrappleDistance) {

					// Check if this new hit is closer than the previous
					// hit. If it is, the current marker will be the
					// new closest marker.
					if ((i == 0) || (distance < lastDistance)) {

						closestMarker = markers [i];
						lastDistance = distance; // save
					}
				}
			}

			return closestMarker;
		}

		private class ScopeViewScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
		{
			private readonly ScopeView _view;

			public ScopeViewScaleListener(ScopeView view)
			{
				_view = view;
			}

			public override bool OnScale(ScaleGestureDetector detector)
			{
//				_view._scaleFactor *= detector.ScaleFactor;
//
//				// put a limit on how small or big the image can get.
//				if (_view._scaleFactor > 5.0f)
//				{
//					_view._scaleFactor = 5.0f;
//				}
//				if (_view._scaleFactor < 0.1f)
//				{
//					_view._scaleFactor = 0.1f;
//				}
//
//				_view.Invalidate();
				return true;
			}
		}
	}
}

