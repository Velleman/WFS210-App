using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace WFS210.UI
{
	public class Marker
	{
		protected int VariablePosition{ get; set;}
		protected UIImage Image{ get; set;}

		public Marker (string resourceUrl)
		{
			Image = UIImage.FromFile (resourceUrl);
		}
	}
}

