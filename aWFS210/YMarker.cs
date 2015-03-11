using System;
using Android.App;
using Android.Content;

namespace WFS210.Android
{
	public class YMarker : Marker
	{
		public YMarker (Context context,int resId,float width,float value) : base(context,resId,MarkerLayout.Horizontal)
		{
			CalculateBounds (width);
			Value = value;
		}
	}
}

