using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace WFS210.UI
{
	public class ScopeDataView : UIView
	{
		public CGPath Path {get; set;}
		public ScopeDataView ()
		{  
			Path = new CGPath ();
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
				g.AddPath (Path);		
				g.DrawPath (CGPathDrawingMode.Stroke);

			}       
		}

	}
}

