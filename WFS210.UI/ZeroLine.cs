using System;
using MonoTouch.CoreAnimation;
using System.Drawing;

namespace WFS210.UI
{
	public class ZeroLine : Marker
	{
		public ZeroLine (string resource,int pos,string name) : base(resource,name)
		{
			Position = new PointF(Image.CGImage.Width/2+1 ,pos);
		}

		public int Y
		{
			set
			{
				var pos = Position;
				pos.X = value;
				Position = pos;
			}
		}
	}
}

