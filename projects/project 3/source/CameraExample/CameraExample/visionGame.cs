﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CameraExample
{
    [Activity(Label = "VisionGame")]
    public class VisionGame : Activity
    {
        // Creates Image object
        Image image = new Image();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.GameView);

            // TODO: Make text change when progress bar fills up.
            TextView wordToFind = FindViewById<TextView>(Resource.Id.gameText);
            //make method to grab word
            wordToFind.Text = image.GetWords(word_track);

            if (IsThereAnAppToTakePictures() == true)
            {
                Button cam = FindViewById<Button>(Resource.Id.launchCameraButton);
                cam.Click += TakePicture;
            }

            Button submit = FindViewById<Button>(Resource.Id.btn_submit);
            submit.Click += image.SubmitPic();
        }

      
        /// <summary>
        /// Apparently, some android devices do not have a camera.  To guard against this,
        /// we need to make sure that we can take pictures before we actually try to take a picture.
        /// </summary>
        /// <returns></returns>
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities
                (intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        // Intent to take a picture
        private void TakePicture(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }

        // Hopefully saves the bitmap
        //private void SaveBitmap(Bitmap map)
        //{
        //    System.IO.FileStream fs = new System.IO.FileStream(_file.Path, System.IO.FileMode.OpenOrCreate);
        //    map.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 85, fs);
        //    var stream = new FileStream(filePath, FileMode.Create);
        //    map.Compress(Bitmap.CompressFormat.Png, 100, stream);
        //    fs.Flush();
        //    fs.Close();
        //}

        // <summary>
        // Called automatically whenever an activity finishes
        // </summary>
        // <param name = "requestCode" ></ param >
        // < param name="resultCode"></param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Saves bitmap to image class
            image.SetBitmap((Android.Graphics.Bitmap)data.Extras.Get("data"));

            // Hopefully saves bitmap to memory
           // SaveBitmap(bitmap);

            // Sets image on GameView layout
            ImageView takenPic = FindViewById<ImageView>(Resource.Id.gameImage);
            if (image.CheckBitmap() != null)
            {
                takenPic.SetImageBitmap(image.GetBitmap());
            }

            // Dispose of the Java side bitmap.
            System.GC.Collect();

            Finish();
        }

       

        protected void onStart()
        {
        
        }

        protected void onRestart()
        {

        }

        protected void onResume()
        {
        
        }

        protected void onPause()
        {
        
        }

        protected void onStop()
        {
        
        }

        protected void onDestroy()
        {
        
        }
    }
}