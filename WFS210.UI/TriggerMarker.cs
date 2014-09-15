using System;
using MonoTouch.CoreAnimation;
using MonoTouch.UIKit;
using System.Drawing;

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

