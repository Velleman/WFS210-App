//
// Grid.cs
//
// Author: Brecht Nuyttens
//
// Copyright (c) 2015 Velleman nv
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Android.Graphics;

namespace WFS210.Droid
{
	public class Grid
	{
		private int _width;
		private int _height;
		private int _widthOffSet;
		private int _heigthOffSet;
		private float _horizontalDivs;

		public float StartWidth{
			get{
				return _widthOffSet;
			}
		}

		public float EndWidth{
			get{
				return _width;
			}
		}

		public float Width{
			get{
				return _width - _widthOffSet;
			}
		}

		public float StartHeight{
			get{
				return _heigthOffSet;
			}
		}

		public float EndHeight{
			get{
				return _height - _heigthOffSet;
			}
		}

		public float Height{
			get{
				return _height - _heigthOffSet;
			}
		}

		public float HorizontalDivs
		{
			get{
				return _horizontalDivs;
			}
		}

		private float _rasterSpace;

		public Grid (int width,int height,int widthOffSet,int heightOffSet)
		{
			_width = width;
			_widthOffSet = widthOffSet;
			_height = height;
			_heigthOffSet = heightOffSet;
			_rasterSpace = (height - 2 * heightOffSet) / 10;
			_horizontalDivs = _width / _rasterSpace;
		}

		public void Draw(Canvas canvas,Paint paint)
		{

			//Top line
			canvas.DrawLine (StartWidth, StartHeight, EndWidth, StartHeight, paint);
			//Left line
			canvas.DrawLine (StartWidth, StartHeight, StartWidth, EndHeight, paint);
			//Right line
			//canvas.DrawLine (width-1, offSet, width-1, height, paintGrid);
			//Bottom line
			canvas.DrawLine (StartWidth, EndHeight, EndWidth, EndHeight, paint);

			canvas.DrawLine (StartWidth, StartHeight + ((EndHeight - StartHeight ) / 2f), EndWidth, (float)StartHeight + ((EndHeight-StartHeight) / 2f), paint);
			for (int i = 0; i < 5; i++) {
				canvas.DrawLine (StartWidth, StartHeight + ((EndHeight - StartHeight ) / 2f) + (_rasterSpace * i), EndWidth, (float)StartHeight + ((EndHeight - StartHeight ) / 2f) + (_rasterSpace * i), paint);
				canvas.DrawLine (StartWidth, StartHeight + ((EndHeight - StartHeight ) / 2f) - (_rasterSpace * i), EndWidth, (float)StartHeight + ((EndHeight - StartHeight ) / 2f) - (_rasterSpace * i), paint);
			}
			var distance = _rasterSpace + StartWidth;
			while (distance < EndWidth) {
				canvas.DrawLine (distance, StartHeight, distance, EndHeight, paint);
				distance += _rasterSpace;
			}

		}
	}
}

