using System;
using Android.App;
using Android.Graphics;

namespace WFS210.Android
{
	public class ZeroLine : Marker
	{
		public ZeroLine (Activity activity,int resId,int width,int value) : base(activity,resId)
		{
			Layout = MarkerLayout.Horizontal;
			CalculateBounds (width);
			Value = value;
		}
	}
}

