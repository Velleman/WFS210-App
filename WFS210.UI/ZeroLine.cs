using System;
using CoreAnimation;
using CoreGraphics;

namespace WFS210.UI
{
	public class ZeroLine : Marker
	{
		public ZeroLine (string resource,int pos,string name,int inlay) : base(resource, name, MarkerLayout.Horizontal,inlay)
		{
			Value = pos;
		}
	}
}

