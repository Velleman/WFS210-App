using System;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;
using System.Drawing;

namespace WFS210.UI
{
	public class TriggerMarker : Marker
	{
		public TriggerMarker (string resource,int pos,string name) : base(resource,name)
		{
			Position = new PointF(Image.CGImage.Width/2 - 18 ,pos);
		}

		public int TriggerLevel
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

