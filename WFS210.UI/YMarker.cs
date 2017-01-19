using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;

namespace WFS210.UI
{
	public class YMarker : Marker
	{
		public YMarker (string resource,int pos,string name,int inlay) : base(resource, name, MarkerLayout.Horizontal,inlay)
		{
			//Position = new PointF(Image.CGImage.Width/2 - 18,pos);
			Value = pos;
		}


	}
}

