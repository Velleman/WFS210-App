using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace WFS210.UI
{
	public class CalibrationIndicator : UITextField
	{
		/// <summary>
		/// Initializes a new instance of the ${DeclaringType} ${DeclaringTypeKind}.
		/// </summary>
		public CalibrationIndicator () : base ()
		{
			Layer.Bounds = new CGRect (0, 00, 200, 100);
			Layer.Position = new CGPoint( 450, 500);
			Layer.ContentsGravity = CALayer.GravityResizeAspect;
			Layer.BorderWidth = 3f;
			Layer.CornerRadius = 20;
			Layer.BorderColor = UIColor.Gray.CGColor;
			Text = "Calibrating...";
			Font = UIFont.FromName("Helvetica-Bold", 30f);
			TextAlignment = UITextAlignment.Center;
			VerticalAlignment = UIControlContentVerticalAlignment.Center;
			BackgroundColor = UIColor.Gray;
		}
	}
}

