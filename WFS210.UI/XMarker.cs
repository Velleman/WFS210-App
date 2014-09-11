using System;
using MonoTouch.CoreAnimation;
using System.Drawing;
using MonoTouch.UIKit;

namespace WFS210.UI
{
	public class XMarker : Marker
	{

		public XMarker (string resource, int pos, string name) : base(resource, name, MarkerLayout.Vertical)
		{
			Position = new PointF(pos , Image.CGImage.Height /2 - 18);
		}

		public int X
		{
			set
			{
				var pos = Position;
				pos.X = value;
				Position = pos;
			}
			get{
				return Convert.ToInt32(Position.X);
			}
		}
	}
}

