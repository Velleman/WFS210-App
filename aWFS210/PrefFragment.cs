//
// PreferenceFragment.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace WFS210.Droid
{
	public class PrefFragment : PreferenceFragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			AddPreferencesFromResource (Resource.Xml.preferences);

			for(int i=0; i < PreferenceScreen.PreferenceCount; i++)
			{
				if(PreferenceScreen.GetPreference(i) is PreferenceCategory)
				{
					PreferenceCategory pc = (PreferenceCategory) PreferenceScreen.GetPreference(i);
					if(pc.Title.ToString() == "Versions")
					{
						ISharedPreferences sp = PreferenceManager.SharedPreferences;
						Preference p = pc.GetPreference(0);
						p.Summary = sp.GetString("VERSIONNUMBERSCOPE", "SCOPE VERSION NOT FOUND");
						Preference p2 = pc.GetPreference(1);
						p2.Summary = sp.GetString("VERSIONNUMBERWIFI", "WIFI VERSION NOT FOUND");
						Preference p3 = pc.GetPreference(2); 
						p3.Summary = sp.GetString("APPVERSION", "APP VERSION NOT FOUND");
					}
					if(pc.Title.ToString() == "Settings")
					{
						ISharedPreferences sp = PreferenceManager.SharedPreferences;
						Preference p = pc.GetPreference(0);
						p.Summary = sp.GetString("WIFINAME", "");
					}
					if(pc.Title.ToString() == "Calibration")
					{
						ISharedPreferences sp = PreferenceManager.SharedPreferences;
						Preference p = pc.GetPreference(0);
					}

				}
			}
		}
	}
}

