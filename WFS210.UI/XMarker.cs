using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace WFS210.UI
{
	public class XMarker : Marker
	{

		public XMarker (string resource, int pos, string name,int inlay) : base(resource, name, MarkerLayout.Vertical,inlay)
		{
			Value = pos;
		}
	}
}

