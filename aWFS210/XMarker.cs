using System;
using Android.App;
using Android.Graphics;

namespace WFS210.Android
{
	public class XMarker : Marker
	{
		public XMarker (Activity activity,int resId,int height,int value) : base(activity,resId)
		{
			Layout = MarkerLayout.Vertical;
			CalculateBounds (height);
			Value = value;
		}
	}
}

