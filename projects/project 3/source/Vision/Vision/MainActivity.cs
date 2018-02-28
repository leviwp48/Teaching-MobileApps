using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Provider;
using Android.Graphics;
using System;
using System.IO;


namespace Vision
{
    [Activity(Label = "MainActivity", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            FindViewById<Button>(Resource.Id.btn_game).Click += start_game;
        }

        private void start_game(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(this, typeof(VisionGame));
            this.StartActivity(intent);
        }
    }
}