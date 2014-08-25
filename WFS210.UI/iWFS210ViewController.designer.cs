// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace WFS210.UI
{
	[Register ("iWFS210ViewController")]
	partial class iWFS210ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		ScopeView ScopeView { get; set; }

		[Action ("UIButton6_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton6_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (ScopeView != null) {
				ScopeView.Dispose ();
				ScopeView = null;
			}
		}
	}
}
