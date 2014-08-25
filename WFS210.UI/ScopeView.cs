using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreAnimation;
using WFS210;

namespace WFS210.UI
{
	public partial class ScopeView : UIView
	{
		CGPath path;
		PointF initialPoint;
		PointF latestPoint;
		Oscilloscope wfs210;
		PointF[] scopePoints;
		int width, height;
		float sampleToPointRatio;

		Trace trace1;
		Trace trace2;
		Grid grid;
		XMarker xMarker1;
		XMarker xMarker2;
		YMarker yMarker1;
		YMarker yMarker2;

		/// <summary>
		/// Initializes a new instance of the <see cref="iWFS210.ScopeView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ScopeView (IntPtr handle) : base (handle)
		{
			BackgroundColor = UIColor.Gray;

			path = new CGPath ();
			RectangleF frame = this.Frame;
			width = (int)frame.Width;
			height = (int)frame.Height;
			grid = new Grid (frame);
		}

		/// <summary>
		/// Sets the oscilloscope.
		/// </summary>
		/// <param name="scope">Scope.</param>
		public void setOscilloscope (Oscilloscope scope)
		{
			wfs210 = scope;
			sampleToPointRatio = (float)height / (wfs210.DeviceContext.SampleMax - wfs210.DeviceContext.SampleMin);
		}

		/// <summary>
		/// Updates the scope view.
		/// </summary>
		public void UpdateScopeView ()
		{
			SampleBuffer buffer = wfs210.Channels [0].Samples;
			scopePoints = new PointF[width];
			for (int x = 0; x < width; x++) {
				scopePoints [x].X = x;
				scopePoints [x].Y = MapSampleDataToScreen (buffer [x]);
			}
			path.AddLines (scopePoints);
			initialPoint.X = 0;
			initialPoint.Y = buffer [0];
			latestPoint.X = width - 1;
			latestPoint.Y = buffer [width - 1];
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
				DrawGrid (g);
			}       
		}

		public void DrawTraces ()
		{

		}

		public void DrawTrace (Trace trace)
		{

		}

		public void DrawGrid (CGContext g)
		{
			//get graphics context


			//set up drawing attributes
			g.SetLineWidth (1);
			UIColor.Gray.SetStroke ();
			g.SetAlpha (0.5f);
			//add geometry to graphics context and draw it
			foreach (CGPath path in grid.HorizontalLines) {
				g.AddPath (path);
			}
			foreach (CGPath path in grid.VerticalLines) {
				g.AddPath (path);
			}
			g.DrawPath (CGPathDrawingMode.Stroke);
		}

		public void DrawMarkers ()
		{

		}

		public void DrawMarker (Marker marker)
		{

		}
	}
}