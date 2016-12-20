using System;
using Android.Graphics.Drawables;
using Android.Content.Res;
using Android.Content;
using Android.App;
using Android.Graphics;
using Android.Widget;
using Android.Animation;


namespace WFS210.Droid
{
	public class Marker
	{
		protected NinePatchDrawable npd;
		private Rect _bounds;
		private MarkerLayout _markerLayout;

		protected int _value;

		public Marker (Context context, int resourceId, MarkerLayout ml)
		{
			_markerLayout = ml;
			npd = (NinePatchDrawable)context.Resources.GetDrawable (resourceId);
			_bounds = new Rect (0, 0, npd.IntrinsicWidth, npd.IntrinsicHeight);
		}

		public MarkerLayout Layout {
			get {
				return _markerLayout;
			}
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public float Value {
			get{ return _value; }
			set { 

				if (_markerLayout == MarkerLayout.Horizontal) {
					ValueAnimator animator = ValueAnimator.OfInt (new[]{ _value, (int)value });
					animator.SetDuration (100);
					animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) => {
						_bounds.Top = (int)e.Animation.AnimatedValue - npd.IntrinsicHeight / 2;
						_bounds.Bottom = _bounds.Top + npd.IntrinsicHeight;
					};
					animator.Start ();

				} else {
					ValueAnimator animator = ValueAnimator.OfInt (new[]{ _value, (int)value });
					animator.SetDuration (100);
					animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) => {
						_bounds.Left = (int)e.Animation.AnimatedValue - npd.IntrinsicWidth / 2;
						_bounds.Right = _bounds.Left + npd.IntrinsicWidth;
					};
					animator.Start ();
				}
				_value = (int)value;
			}
		}

		protected void CalculateBounds (float size)
		{
			if (_markerLayout == MarkerLayout.Horizontal) {
				_bounds.Right = (int)size;
			} else {
				_bounds.Bottom = (int)size;
			}
		}

		protected void CalculateBounds (float size, float offset)
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

