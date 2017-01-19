using System;

namespace WFS210.UI
{
	public struct Padding
	{
		/// <summary>
		/// Left padding.
		/// </summary>
		public int Left;

		/// <summary>
		/// Right padding.
		/// </summary>
		public int Right;

		/// <summary>
		/// Top padding.
		/// </summary>
		public int Top;

		/// <summary>
		/// Bottom padding.
		/// </summary>
		public int Bottom;

		/// <summary>
		/// Initializes a new instance of the <see cref="WFS210.UI.Padding"/> class.
		/// </summary>
		/// <param name="left">Left padding.</param>
		/// <param name="right">Right padding.</param>
		/// <param name="top">Top padding.</param>
		/// <param name="bottom">Bottom padding.</param>
		public Padding(int left, int right, int top, int bottom)
		{
			this.Left = left;
			this.Right = right;
			this.Top = top;
			this.Bottom = bottom;
		}

		/// <summary>
		/// Gets the total amount of horizontal padding.
		/// </summary>
		/// <value>The amount of horizontal padding.</value>
		public int Horizontal {
			get {
				return Left + Right;
			}
		}

		/// <summary>
		/// Gets the total amount of vertical padding.
		/// </summary>
		/// <value>The amount of vertical padding.</value>
		public int Vertical {
			get {
				return Top + Bottom;
			}
		}
	}
}

