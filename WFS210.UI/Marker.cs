using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;

namespace WFS210.UI
{
	public class Marker
	{
		public MarkerLayout Layout { get; set; }

		protected UIImage Image { get; set; }

		public string Name { get; set; }

		public CALayer Layer { get; set; }

		public Marker (string resourceUrl, string name, MarkerLayout layout)
		{
			this.Layout = layout;

			Image = UIImage.FromFile (resourceUrl);

			Name = name;

			Layer = new CALayer ();
			Layer.Bounds = new RectangleF (0, 0, Image.CGImage.Width, Image.CGImage.Height );
			Layer.Position = Position;
			Layer.Contents = Image.CGImage;
		}

		public PointF Position
		{
			get
			{
				return Layer.Position;
			}
			set
			{ 
				Layer.Position = value;
			}
		}
	}
}

