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
		Trace[] traces = new Trace[2];
		float sampleToPointRatio;


		CALayer gridLayer;
		/// <summary>
		/// Initializes a new instance of the <see cref="iWFS210.ScopeView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ScopeView (IntPtr handle) : base (handle)
		{
			BackgroundColor = UIColor.Gray;

			path = new CGPath ();
			RectangleF frame = this.Frame;

			gridLayer = new CALayer ();
			gridLayer.Bounds = new RectangleF (0, 0, frame.Width, frame.Height);
			gridLayer.Position = new PointF (frame.Width/2,frame.Height/2);
			gridLayer.Contents = UIImage.FromFile ("VIEWPORT/VIEWPORT-130x78.png").CGImage;
			//gridLayer.ContentsGravity = CALayer.GravityResizeAspect;
			gridLayer.ZPosition = 0;
			Layer.AddSublayer (gridLayer);
			this.Layer.ZPosition = 10;
		}
			
		public Oscilloscope Wfs210
		{
			set{

				wfs210 = value;
				sampleToPointRatio = (float)this.Frame.Height / (wfs210.DeviceContext.SampleMax - wfs210.DeviceContext.SampleMin);
			}	
		}

		/// <summary>
		/// Updates the scope view.
		/// </summary>
		public void UpdateScopeView ()
		{
			SampleBuffer buffer = wfs210.Channels [0].Samples;
			scopePoints = new PointF[buffer.Count];
			for (int x = 0; x < this.Frame.Width; x++) {
				scopePoints [x].X = x;
				scopePoints [x].Y = MapSampleDataToScreen (buffer [x]);
			}
			path.AddLines (scopePoints);
			initialPoint.X = 0;
			initialPoint.Y = buffer [0];
			latestPoint.X = this.Frame.Width - 1;
			latestPoint.Y = buffer [(int)this.Frame.Width - 1];
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


		}

		public void DrawMarkers ()
		{

		}

		public void DrawMarker (Marker marker)
		{

		}
	}
}