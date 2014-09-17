using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;

namespace WFS210.UI
{
	public class Marker
	{
		public MarkerLayout Layout { get; set; }

		public UIImage Image { get; set; }

		public CALayer Layer { get; set;}

		public string Name { get; set; }

		protected int value;

		public int Inlay { get; set;}

		public Marker (string resourceUrl, string name, MarkerLayout layout, int inlay)
		{
			this.Layout = layout;

			Image = UIImage.FromFile (resourceUrl);

			Layer = new CALayer ();
			Layer.Contents = Image.CGImage;
			Layer.Bounds = new RectangleF (0, 0, Image.CGImage.Width, Image.CGImage.Height);

			Name = name;

			Inlay = inlay;

		}

		public int Value
		{
			get{ return value; }
			set{ 
				this.value = value;
				if (this.Layout == MarkerLayout.Horizontal)
					Position = new PointF (Layer.Position.X, value);
				else
					Position = new PointF (value, Layer.Position.Y);
			}
		}

		public PointF Position
		{
			set{ 
				CATransaction.Begin ();
				Layer.Position = value;
				Layer.RemoveAnimation ("position");
				CATransaction.Commit ();
			}
		}
	}
}

