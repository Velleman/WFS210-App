using System;
using CoreAnimation;
using UIKit;
using CoreGraphics;

namespace WFS210.UI
{
	public class TriggerMarker : Marker
	{
		public TriggerMarker (string resource,int pos,string name,int inlay) : base(resource, name, MarkerLayout.Horizontal,inlay)
		{
			Value = pos;
		}
	}
}

