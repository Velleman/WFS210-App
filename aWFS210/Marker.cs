using System;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Content;
using Android.App;
using Android.Graphics;


namespace WFS210.Android
{
	public class Marker
	{
		protected NinePatchDrawable npd;

		public MarkerLayout Layout { get; set; }

		protected int value;

		public Marker (Activity activity,int resourceId)
		{
			npd = (NinePatchDrawable)activity.Resources.GetDrawable (resourceId);
		}

		public virtual void Draw(Canvas canvas)
		{
			npd.Draw (canvas);
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public int Value
		{
			get{ return value; }
			set{ 
				this.value = value;
				SetValue ();
			}
		}

		protected void CalculateBounds(int size)
		{
			if (Layout == MarkerLayout.Horizontal) {
				var left = 0;
				var top = 0;
				var right = size;
				var bottom = top + npd.IntrinsicHeight;
				npd.Bounds = new Rect (left, top, right, bottom);
			} else {
				var left = 0 - (npd.IntrinsicWidth/2);
				var top = 0;
				var right = left + npd.IntrinsicWidth;
				var bottom = size;
				npd.Bounds = new Rect (left, top, right, bottom);
			}
		}

		private void SetValue(){
			if (Layout == MarkerLayout.Horizontal) {
				var left = -20;
				var top = this.value - (npd.IntrinsicHeight/2);
				var right = npd.Bounds.Right;
				var bottom = top + npd.IntrinsicHeight;
				npd.Bounds = new Rect (left, top, right, bottom);
			} else {
				var left = this.value - (npd.IntrinsicWidth/2);
				var top = 0;
				var right = left + npd.IntrinsicWidth;
				var bottom = npd.Bounds.Bottom;
				npd.Bounds = new Rect (left, top, right, bottom);
			}
		}
	}
}

