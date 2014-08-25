using System;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace WFS210.UI
{
	public class Grid
	{
		float sizePerDiv;
		CGPath[] horizontalLines = new CGPath[10];
		CGPath[] verticalLines;
		RectangleF frame;
		/// <summary>
		/// Gets the horizontal lines.
		/// </summary>
		/// <value>The horizontal lines.</value>
		public CGPath[] HorizontalLines
		{
			get{
				return horizontalLines;
			}
		}
		/// <summary>
		/// Gets the vertical lines.
		/// </summary>
		/// <value>The vertical lines.</value>
		public CGPath[] VerticalLines
		{
			get{
				return verticalLines;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.Grid"/> class.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public Grid (RectangleF rect)
		{
			frame = rect;
			for(int i=0;i<10;i++)
			{
				horizontalLines [i] = new CGPath ();
			}
			CalculateSizePerDiv (frame);
			GenerateHorizontalLines (sizePerDiv);
			GenerateVerticalLines (sizePerDiv);
		}

		/// <summary>
		/// Calculates the size per div.
		/// </summary>
		/// <param name="rect">Rect.</param>
		void CalculateSizePerDiv (RectangleF rect)
		{
			sizePerDiv = rect.Height / 10;
		}
		/// <summary>
		/// Generates the horizontal lines.
		/// </summary>
		/// <param name="sizePerDiv">Size per div.</param>
		void GenerateHorizontalLines (float sizePerDiv)
		{
			float rest = frame.Height % 10;
			float spareSpace = rest / 2;
			for (int i = 0; i < 10; i++) {
				PointF[] line = new PointF[2];

				line [0].X = 0;
				line[0].Y = (i * sizePerDiv) + spareSpace;
				line [1].X = frame.Width;
				line[1].Y = (i * sizePerDiv) + spareSpace;
				horizontalLines [i].AddLines (line);
			}
		}
		/// <summary>
		/// Generates the vertical lines.
		/// </summary>
		/// <param name="sizePerDiv">Size per div.</param>
		void GenerateVerticalLines (float sizePerDiv)
		{
			int timeDivisions = (int)(Math.Ceiling(frame.Width / sizePerDiv));
			verticalLines = new CGPath[timeDivisions];
			for(int i=0;i<timeDivisions;i++)
			{
				verticalLines [i] = new CGPath ();
			}
			for (int i = 0; i < timeDivisions; i++) {
				PointF[] line = new PointF[2];

				line [0].X = (i * sizePerDiv);
				line [0].Y = 0;
				line [1].X = (i * sizePerDiv);
				line[1].Y =  frame.Width;
				verticalLines [i].AddLines (line);
			}
		}
	}
}

