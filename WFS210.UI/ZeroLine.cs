using System;
using MonoTouch.CoreAnimation;
using System.Drawing;

namespace WFS210.UI
{
	public class ZeroLine : Marker
	{
		public CALayer Layer{ get; set;}
		PointF position;
		public ZeroLine (string resource,int pos) : base(resource)
		{
			position = new PointF(Image.CGImage.Width/2+1 ,pos);
			Layer = new CALayer ();
			Layer.Bounds = new RectangleF (0, 0, Image.CGImage.Width, Image.CGImage.Height);
			Layer.Position = position;
			Layer.Contents = Image.CGImage;
		}

		public int Y
		{
			set
			{
				base.VariablePosition = value;
				position.X = base.VariablePosition;
			}
		}
	}
}

