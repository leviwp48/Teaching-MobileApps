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
 Save bitmap to files and into gallery
*/
namespace CameraExample
{
    [Activity(Label = "CameraExample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {            
        public static Bitmap bitmap;
        public static Bitmap copy_bitmap;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures() == true)
            {       
                FindViewById<Button>(Resource.Id.launchCameraButton).Click += TakePicture;
            }


           // FindViewById<Button>(Resource.Id.btn_game).Click += send_image;
                
            
        }

        //private void send_image(object sender, EventArgs e)
        //{
        //    Intent send_data = new Intent(this, typeof(VisionGame));
        //    Bundle extras = new Bundle();
        //    extras.PutParcelable("Image", copy_bitmap);
        //    Intent.PutExtras(extras);
        //    StartActivity(send_data);
        // }

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

        private void TakePicture(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);           
            StartActivityForResult(intent, 0);
        }

        private void ExportBitmapAsPNG(Bitmap bitmap)
        {      
            System.IO.FileStream fs = new System.IO.FileStream(_file.Path, System.IO.FileMode.OpenOrCreate);
            bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 85, fs);
            var stream = new FileStream(filePath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            fs.Flush();
            fs.Close();
        }

        // <summary>
        // Called automatically whenever an activity finishes
        // </summary>
        // <param name = "requestCode" ></ param >
        // < param name="resultCode"></param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);            

            //AC: workaround for not passing actual files
            Android.Graphics.Bitmap bitmap = (Android.Graphics.Bitmap)data.Extras.Get("data");

            //scale image to make manipulation easier
            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);

            ImageView takenPic = FindViewById<ImageView>(Resource.Id.takenPictureImageView);
            if (copy_bitmap != null)
            {
                takenPic.SetImageBitmap(copy_bitmap);
            }

            var fileName = GetFileName(_dir);
            using (var os = new FileStream(fileName, ParcelFileMode.CreateNew))
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 95, os);
            }

                // Dispose of the Java side bitmap.
                System.GC.Collect();
        }       
    }
}

