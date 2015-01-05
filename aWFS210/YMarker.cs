using System;
using Android.App;

namespace WFS210.Android
{
	public class YMarker : Marker
	{
		public YMarker (Activity activity,int resId,int width,int value) : base(activity,resId)
		{
			Layout = MarkerLayout.Horizontal;
			CalculateBounds (width);
			Value = value;
		}
	}
}

