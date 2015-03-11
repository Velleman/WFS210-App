using System;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Content;
using Android.App;
using Android.Graphics;
using Android.Widget;


namespace WFS210.Android
{
	public class Marker
	{
		protected NinePatchDrawable npd;
		private Rect _bounds;
		private MarkerLayout _markerLayout;

		protected int _value;

		public Marker (Context context,int resourceId,MarkerLayout ml)
		{
			_markerLayout = ml;
			npd = (NinePatchDrawable)context.Resources.GetDrawable (resourceId);
			_bounds = new Rect (0, 0, npd.IntrinsicWidth, npd.IntrinsicHeight);
		}

		public MarkerLayout Layout{
			get{
				return _markerLayout;
			}
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public float Value
		{
			get{ return _value; }
			set{ 
				_value = (int)value;
				if (_markerLayout == MarkerLayout.Horizontal) {
					_bounds.Top = _value - npd.IntrinsicHeight/2;
					_bounds.Bottom = _bounds.Top + npd.IntrinsicHeight;
				} else {
					_bounds.Left = _value - npd.IntrinsicWidth /2;
					_bounds.Right = _bounds.Left + npd.IntrinsicWidth;
				}
			}
		}

		protected void CalculateBounds(float size)
		{
			if (_markerLayout == MarkerLayout.Horizontal) {
				_bounds.Right = (int)size;
			} else {
				_bounds.Bottom = (int)size;
			}
		}

		protected void CalculateBounds(float size,float offset)
		{
			if (_markerLayout == MarkerLayout.Horizontal) {
				_bounds.Left = (int)offset;
				_bounds.Right = (int)size;
			} else {
				_bounds.Top = (int)offset;
				_bounds.Bottom = (int)size;
			}
		}
			
		public void Draw (Canvas canvas)
		{
			npd.Bounds = _bounds;
			npd.Draw (canvas);
		}
	}
}

