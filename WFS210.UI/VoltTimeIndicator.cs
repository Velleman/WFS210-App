using System;
using MonoTouch.CoreAnimation;
using System.Drawing;
using MonoTouch.UIKit;

namespace WFS210.UI
{
	public class VoltTimeIndicator : UITextField
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.VoltTimeIndicator"/> class.
		/// </summary>
		public VoltTimeIndicator () : base ()
		{
			Layer.Bounds = new RectangleF (0, 00, 200, 100);
			Layer.Position = new PointF( 150, 100);
			Layer.ContentsGravity = CALayer.GravityResizeAspect;
			Layer.BorderWidth = 3f;
			Layer.CornerRadius = 20;
			Layer.BorderColor = UIColor.Gray.CGColor;
			Text = "test";
			Font = UIFont.FromName("Helvetica-Bold", 30f);
			TextAlignment = UITextAlignment.Center;
			VerticalAlignment = UIControlContentVerticalAlignment.Center;
			BackgroundColor = UIColor.Gray;
		}
	}
}

