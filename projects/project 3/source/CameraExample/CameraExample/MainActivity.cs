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

/*
 TODO: 
    Get bitmap data to move to activity
    Get game to use camera button
    Get progress bar working
*/
namespace CameraExample
{
    [Activity(Label = "CameraExample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {            
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            FindViewById<Button>(Resource.Id.btn_game).Click += start_game;

            // FindViewById<Button>(Resource.Id.btn_game).Click += send_image;
        }

        private void start_game(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(VisionGame);
            StartActivity(VisionGame);
        }
        //private void send_image(object sender, EventArgs e)
        //{
        //    Intent send_data = new Intent(this, typeof(VisionGame));
        //    Bundle extras = new Bundle();
        //    extras.PutParcelable("Image", copy_bitmap);
        //    send_data.PutExtras("map", bitmap);
        //    this.StartActivity(send_data);
        //}     
    }
}

