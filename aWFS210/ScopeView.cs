
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

namespace aWFS210
{
	public class ScopeView : View
	{
		public ServiceManager ServiceManager{ get; set;}

		public Oscilloscope Oscilloscope{ get; set;}

		public XMarker[] XMarkers = new XMarker[2];
		public YMarker[] YMarkers = new YMarker[2];
		public ZeroLine[] ZeroLines = new ZeroLine[2];

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

		public void Initialize ()
		{
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
		}

		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged (w, h, oldw, oldh);
			width = w;
			height = h;
			CalculateRatios ();
		}

		/// <param name="canvas">the canvas on which the background will be drawn</param>
		/// <summary>
		/// Implement this to do your drawing.
		/// </summary>
		protected override void OnDraw (Android.Graphics.Canvas canvas)
		{
			base.OnDraw (canvas);

			for(int i=0;i < 2;i++)
			{
				traces [i].Reset ();
				traces[i].MoveTo (0, MapSampleDataToScreen (Oscilloscope.Channels [i].Samples [0]));

			for (int j = 0; j < MapXPosToScreen(TotalSamples); j++) {
					traces[i].LineTo(MapXPosToScreen(j), MapSampleDataToScreen (Oscilloscope.Channels [i].Samples [j]));
			}

				canvas.DrawPath (traces[i], paints[i]);
			}

			DrawGrid (canvas);
		}

		private void DrawGrid(Android.Graphics.Canvas canvas)
		{
			canvas.DrawLine (0, height / 2, width, height / 2, paintGrid);
			for (int i = 0; i < 5; i++) {
				canvas.DrawLine (0, height / 2 + (RasterSpace * i), width, height / 2 + (RasterSpace * i), paintGrid);
				canvas.DrawLine (0, height / 2 - (RasterSpace * i), width, height / 2 - (RasterSpace * i), paintGrid);
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
			TotalSamples = Oscilloscope.DeviceContext.SamplesPerTimeBase * Oscilloscope.DeviceContext.Divisions;
		}

		/// <summary>
		/// Calculates the raster.
		/// </summary>
		private void CalculateRaster()
		{
			RasterSpace = height / Oscilloscope.DeviceContext.Divisions;
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
		private int MapScreenDataToScopeData (int value)
		{
			return (int)((value - Height) / SampleToPointRatio);
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
			var ratio = Width / TotalSamples;
			return (int)(pos * ratio);
		}
	}
}

