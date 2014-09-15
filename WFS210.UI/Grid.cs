using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;

namespace WFS210.UI
{
	public class Grid
	{

		public UIImage Image { get; set; }

		public string Name { get; set; }

		protected PointF position;

		public Grid (string resourceUrl, string name)
		{

			Image = UIImage.FromFile (resourceUrl);

			Name = name;
		}

		public PointF Position
		{
			get{ return position;}
			set{ position = value;}
		}
	}
}

