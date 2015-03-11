using System;
using Android.App;
using Android.Graphics;
using Android.Content;

namespace WFS210.Android
{
	public class XMarker : Marker
	{
		public XMarker (Context context,int resId,int height,int value) : base(context,resId,MarkerLayout.Vertical)
		{
			CalculateBounds (height);
			Value = value;
		}
	}
}

