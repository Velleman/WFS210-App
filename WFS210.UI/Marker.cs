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

		public string Name { get; set; }

		protected int value;

		public int Inlay { get; set;}

		public Marker (string resourceUrl, string name, MarkerLayout layout, int inlay)
		{
			this.Layout = layout;

			Image = UIImage.FromFile (resourceUrl);

			Name = name;

			Inlay = inlay;

		}

		public int Value
		{
			get{ return value; }
			set{ 
				this.value = value;
			}
		}
	}
}

