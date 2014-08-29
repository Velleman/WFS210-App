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

		XMarker[] xMarkers = new XMarker[2];
		YMarker[] yMarkers = new YMarker[2];
		ZeroLine[] zeroLines = new ZeroLine[2];
		TriggerMarker trigMarker;


		CALayer gridLayer;
		/// <summary>
		/// Initializes a new instance of the <see cref="iWFS210.ScopeView"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ScopeView (IntPtr handle) : base (handle)
		{
			path = new CGPath ();

			AddGrid ();

			AddXMarkers ();

			AddYMarkers ();

			AddZeroLines ();

			AddTriggerMarker ();
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
			for (int x = 0; x < buffer.Count; x++) {
				scopePoints [x] = new PointF(x,MapSampleDataToScreen (buffer [x]));
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

		private void AddGrid ()
		{
			gridLayer = new CALayer ();
			gridLayer.Bounds = new RectangleF (0, 0, Frame.Width, Frame.Height);
			gridLayer.Position = new PointF (Frame.Width/2,Frame.Height/2);
			gridLayer.Contents = UIImage.FromFile ("VIEWPORT/VIEWPORT-130x78.png").CGImage;
			Layer.AddSublayer (gridLayer);
		}

		private void AddXMarkers()
		{
			//Makeing XMarkers and adding it to the layers
			xMarkers [0] = new XMarker ("MARKERS/MARKER 1 SLIDER-__x60.png",Convert.ToInt32(Frame.Height/1.5));
			xMarkers [1] = new XMarker ("MARKERS/MARKER 2 SLIDER-__x60.png",(int)Frame.Height/2);
			Layer.AddSublayer (xMarkers [0].Layer);
			Layer.AddSublayer (xMarkers [1].Layer);
		}

		private void AddYMarkers()
		{
			//Makeing YMarkers and adding it to the layers
			yMarkers [0] = new YMarker ("MARKERS/MARKER A SLIDER-112x__.png",Convert.ToInt32(Frame.Width/1.5));
			yMarkers [1] = new YMarker ("MARKERS/MARKER B SLIDER-112x__.png",(int)Frame.Width/2);
			Layer.AddSublayer (yMarkers [0].Layer);
			Layer.AddSublayer (yMarkers [1].Layer);
		}

		void AddZeroLines ()
		{
			//Makeing XMarkers and adding it to the layers
			zeroLines [0] = new ZeroLine ("ZEROLINE/ZERO-CHAN1-131x__.png",Convert.ToInt32(Frame.Height/5));
			zeroLines [1] = new ZeroLine ("ZEROLINE/ZERO-CHAN2-131x__.png",(int)Frame.Height/6);
			Layer.AddSublayer (zeroLines [0].Layer);
			Layer.AddSublayer (zeroLines [1].Layer);
		}

		private void AddTriggerMarker()
		{
			//Makeing TriggerMarkers and adding it to the layers
			trigMarker = new TriggerMarker ("TRIGGER LEVEL/TRIG SLIDER-SLOPE UP-112x__.png",Convert.ToInt32(Frame.Height/3));
			Layer.AddSublayer (trigMarker.Layer);
		}
	}
}