using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;

namespace WFS210.UI
{
	public class Marker
	{
		protected int VariablePosition{ get; set;}
		private PointF position;
		protected UIImage Image{ get; set;}
		public string Name{ get; set;}
		public CALayer Layer{ get; set;}
		public Marker (string resourceUrl,string name)
		{
			Image = UIImage.FromFile (resourceUrl);
			Name = name;
			Layer = new CALayer ();
			Layer.Bounds = new RectangleF (0, 0, Image.CGImage.Width, Image.CGImage.Height );
			Layer.Position = Position;
			Layer.Contents = Image.CGImage;
		}

		public PointF Position
		{
			get{ return position;}
			set{ 
				position = value;
				Layer.Position = position;
			}
		}
	}
}

