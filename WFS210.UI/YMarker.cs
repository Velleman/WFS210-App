using System;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using System.Drawing;

namespace WFS210.UI
{
	public class YMarker : Marker
	{
		public YMarker (string resource,int pos,string name) : base(resource, name, MarkerLayout.Horizontal)
		{
			Position = new PointF(Image.CGImage.Width/2 - 18,pos);

		}

		public int Y
		{
			set
			{
				var pos = Position;
				pos.Y = value;
				Position = pos;
			}
		}
	}
}

