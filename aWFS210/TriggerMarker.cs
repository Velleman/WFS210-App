//
// TriggerMarker.cs
//
// Author:
//       Brecht Nuyttens <>
//
// Copyright (c) 2014 Velleman nv
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
using Android.App;
using Android.Graphics.Drawables;
using Android.Content;

namespace WFS210.Android
{
	public class TriggerMarker : Marker
	{
		private bool _slopeUp;
		private NinePatchDrawable _npdSlopeUp, _npdSlopeDown;
		public bool SlopeUp{
			get{
				return _slopeUp;
			}
			set{
				_slopeUp = value;
				if (value) {
					npd = _npdSlopeUp;
				} else {
					npd = _npdSlopeDown;
				}
			}
		}

		public TriggerMarker (Context context,int resIdUp,int resIdDown,float width,float value) : base(context,resIdUp,MarkerLayout.Horizontal)
		{
			_npdSlopeUp = (NinePatchDrawable)context.Resources.GetDrawable (resIdUp);
			_npdSlopeDown = (NinePatchDrawable)context.Resources.GetDrawable (resIdDown);
			CalculateBounds (width);
			Value = value;
		}
	}
}

