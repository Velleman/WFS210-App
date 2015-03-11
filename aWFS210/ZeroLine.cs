using System;
using Android.App;
using Android.Graphics;
using Android.Content;

namespace WFS210.Android
{
	public class ZeroLine : Marker
	{
		public ZeroLine (Context context,int resId,int width,int offSet, int value) : base(context,resId,MarkerLayout.Horizontal)
		{
			CalculateBounds (width,offSet);
			Value = value;
		}
	}
}

