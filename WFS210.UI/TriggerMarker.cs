using System;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;
using System.Drawing;

namespace WFS210.UI
{
	public class TriggerMarker : Marker
	{
		public CALayer Layer{ get; set;}
		PointF position;
		public TriggerMarker (string resource,int pos) : base(resource)
		{
			position = new PointF(Image.CGImage.Width/2 - 18 ,pos);
			Layer = new CALayer ();
			Layer.Bounds = new RectangleF (0, 0, Image.CGImage.Width, Image.CGImage.Height);
			Layer.Position = position;
			Layer.Contents = Image.CGImage;
		}

		public int TriggerLevel
		{
			set
			{
				base.VariablePosition = value;
				position.X = base.VariablePosition;
			}
		}
	}
}

