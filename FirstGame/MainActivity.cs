﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Urho.Droid;
using FirstGame.Core;

namespace FirstGame
{
    [Activity(Label = "FirstGame", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            var mLayout = new AbsoluteLayout(this);
            var surface = UrhoSurface.CreateSurface<Physics.App>(this, new Urho.ApplicationOptions("Data"));
            mLayout.AddView(surface);
            SetContentView(mLayout);
        }

        protected override void OnResume()
        {
            UrhoSurface.OnResume();
            base.OnResume();
        }

        protected override void OnPause()
        {
            UrhoSurface.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory()
        {
            UrhoSurface.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnDestroy()
        {
            UrhoSurface.OnDestroy();
            base.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (!UrhoSurface.DispatchKeyEvent(e))
                return false;
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }
    }

}
