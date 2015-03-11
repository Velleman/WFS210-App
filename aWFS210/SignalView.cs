
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
using Android.Support.V4.View;

namespace WFS210.Android
{
	public class SignalView : View
	{
		public ServiceManager ServiceManager{ get; set;}

		public Oscilloscope Oscilloscope{ get; set;}

		private ScaleGestureDetector _scaleDetector;

		private Rect smallClip,largeClip;

		public Path[] traces;

		public int SelectedChannel { get; set;}

		public event EventHandler<NewDataEventArgs> NewData;

		private Paint[] paints;
		private Paint paintGrid;

		private int offSet;

		private float SampleToPointRatio;
		private int TotalSamples;

		private int GrappleDistance;

		private GestureDetector _gestureDetector;

		public float horizontalDIVS;

		Grid grid;
		 
		List<Marker> _markers;
		Marker _closestMarker;

		private Context _context;
		/// <summary>
		/// Gets or sets the service.
		/// </summary>
		/// <value>The service.</value>
		WFS210.Services.Service Service {
			get {
				return ServiceManager.ActiveService;
			}
		}

		public SignalView (Context context) :
			base (context)
		{
			Initialize (context);
		}

		public SignalView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize (context);
		}

		public SignalView (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize (context);
		}

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

			_markers = new List<Marker> ();

			this.SetBackgroundColor(Color.Transparent);

			offSet = 20;
			//this.SetLayerType (LayerType.Software, null);

			GrappleDistance = 60;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Down:
				_closestMarker = GetMarkerAt (new PointF (e.GetX (), e.GetY ()));
				break;
			case MotionEventActions.Move:
				if (_closestMarker != null) {
					if (_closestMarker is XMarker) {
						var position = _closestMarker.Value;
						var touchPos = e.GetX();
						if (touchPos > grid.StartWidth) {
							if (touchPos < grid.EndWidth)
								position = e.GetX ();
							else
								position = grid.EndWidth;
						} else {
							position = grid.StartWidth;
						}
						_closestMarker.Value = position;
					} else {
						var position = _closestMarker.Value;
						var touchPos = e.GetY();
						if (touchPos > grid.StartHeight) {
							if (touchPos < grid.EndHeight)
								position = e.GetY ();
							else
								position = grid.EndHeight;
						} else {
							position = grid.StartHeight;
						}
						_closestMarker.Value = position;

					}
					Invalidate ();
				}
				break;
			case MotionEventActions.Up:
				_closestMarker = null;
				Invalidate ();
				break;
			default:
				break;
			}
			return true;
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			grid = new Grid (w, h, offSet, offSet);
			smallClip = new Rect ((int)grid.StartWidth,(int)grid.StartHeight+1,(int)grid.EndWidth,(int)grid.EndHeight);
			largeClip = new Rect (0, 0, w, h);
			CalculateRatios ();
			CalculateMarkers ();

		}

		public void Update()
		{
			Invalidate ();
		}

		/// <param name="canvas">the canvas on which the background will be drawn</param>
		/// <summary>
		/// Implement this to do your drawing.
		/// </summary>
		protected override void OnDraw (Canvas canvas)
		{
			base.OnDraw (canvas);

			grid.Draw (canvas,paintGrid);

			canvas.ClipRect (smallClip);

			for(int i=0;i < 2;i++)
			{
				traces [i].Reset ();
				traces[i].MoveTo (grid.StartWidth, MapSampleDataToScreen (Oscilloscope.Channels [i].Samples [0]));

				for (int j = (int)grid.StartWidth; j < MapXPosToScreen(TotalSamples); j++) {
					traces[i].LineTo(MapXPosToScreen(j), MapSampleDataToScreen (Oscilloscope.Channels [i].Samples [j]));
			}

				canvas.DrawPath (traces[i], paints[i]);
			}

			canvas.ClipRect (largeClip,Region.Op.Replace);


			foreach (Marker m in _markers) {
				m.Draw (canvas);
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

		private void CalculateMarkers()
		{
			_markers.Clear ();

			_markers.Add (new ZeroLine (_context,Resource.Drawable.zeroline1,(int)grid.EndWidth,(int)grid.StartWidth,(int)(grid.Height/2.3)));
			_markers.Add (new ZeroLine (_context,Resource.Drawable.zeroline2,(int)grid.EndWidth,(int)grid.StartWidth,(int)(grid.Height/1.7)));

			_markers.Add (new YMarker (_context,Resource.Drawable.markera,grid.EndWidth,(int)(grid.Height/3)));
			_markers.Add (new YMarker (_context,Resource.Drawable.markerb,grid.EndWidth,(int)(grid.Height*0.75)));

			_markers.Add (new TriggerMarker (_context, Resource.Drawable.markertriggerup, Resource.Drawable.markertriggerdown, grid.EndWidth, (int)(grid.Height / 4)));

			_markers.Add (new XMarker (_context,Resource.Drawable.marker1,(int)grid.EndHeight,(int)(grid.Width/3)));
			_markers.Add (new XMarker (_context,Resource.Drawable.marker2,(int)grid.EndHeight,(int)(grid.Width*0.75)));
		}

		/// <summary>
		/// Calculates the ratios.
		/// </summary>
		private void CalculateRatios()
		{
			SampleToPointRatio = (float)((grid.EndHeight - grid.StartHeight)/(Oscilloscope.DeviceContext.UnitsPerDivision * Oscilloscope.DeviceContext.Divisions));
			TotalSamples = (int)(Oscilloscope.DeviceContext.SamplesPerTimeBase * grid.HorizontalDivs);
		}

		/// <summary>
		/// Maps the sample data to screen.
		/// </summary>
		/// <returns>The sample data to screen.</returns>
		/// <param name="sample">Sample.</param>
		private int MapSampleDataToScreen (int sample)
		{
			var result = (int)((sample * SampleToPointRatio));
			return result + (int)grid.StartHeight;
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
			var ratio = (float)grid.Width / (float)TotalSamples;
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
			for (int i = 0; i < _markers.Count; i++) {

				// First, we calculate the distance between the marker
				// line and the touch position. If this distance is within
				// grapple distance, then we have a hit.
				distance = HitTest (pt, _markers [i]);
				if (distance < GrappleDistance) {

					// Check if this new hit is closer than the previous
					// hit. If it is, the current marker will be the
					// new closest marker.
					if ((i == 0) || (distance < lastDistance)) {

						closestMarker = _markers [i];
						lastDistance = distance; // save
					}
				}
			}

			return closestMarker;
		}

	}
}

