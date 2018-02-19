using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using Android.Content.PM;
using Android.Provider;
using Android.Graphics;
using System;


/*
 TODO: 
 Save bitmap to files and into gallery
*/
namespace CameraExample
{
    [Activity(Label = "CameraExample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        /// <summary>
        /// Used to track the file that we're manipulating between functions
        /// </summary>
        public static Java.IO.File _file;        /// <summary>
                                                 /// Used to track the directory that we'll be writing to between functions
                                                 /// </summary>
        public static Java.IO.File _dir;

        public static Bitmap bitmap;
        public static Bitmap copy_bitmap;

        bool[] box_checked = new bool[10];

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures() == true)
            {
                CreateDirectoryForPictures();
                FindViewById<Button>(Resource.Id.launchCameraButton).Click += TakePicture;
            }
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

        /// <summary>
        /// Creates a directory on the phone that we can place our images
        /// </summary>
        private void CreateDirectoryForPictures()
        {
            _dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "CameraExample");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }

        private void TakePicture(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            _file = new Java.IO.File(_dir, string.Format("myPhoto_{0}.jpg", System.Guid.NewGuid()));
            //android.support.v4.content.FileProvider
            //getUriForFile(getContext(), "com.mydomain.fileprovider", newFile);
            //FileProvider.GetUriForFile
            //intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(_file));
            StartActivityForResult(intent, 0);
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

            //grab imageview and display bitmap
            ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);

            //convert bitmap into stream to be sent to Google API
            string bitmapString = "";
            using (var stream = new System.IO.MemoryStream())
            {
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 0, stream);

                var bytes = stream.ToArray();
                bitmapString = System.Convert.ToBase64String(bytes);
            }

            //credential is stored in "assets" folder
            string credPath = "google_api.json";
            Google.Apis.Auth.OAuth2.GoogleCredential cred;

            //Load credentials into object form
            using (var stream = Assets.Open(credPath))
            {
                cred = Google.Apis.Auth.OAuth2.GoogleCredential.FromStream(stream);
            }
            cred = cred.CreateScoped(Google.Apis.Vision.v1.VisionService.Scope.CloudPlatform);

            // By default, the library client will authenticate 
            // using the service account file (created in the Google Developers 
            // Console) specified by the GOOGLE_APPLICATION_CREDENTIALS 
            // environment variable. We are specifying our own credentials via json file.
            var client = new Google.Apis.Vision.v1.VisionService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                ApplicationName = "mobile-apps-tutorial",
                HttpClientInitializer = cred
            });

            //set up request
            var request = new Google.Apis.Vision.v1.Data.AnnotateImageRequest();
            request.Image = new Google.Apis.Vision.v1.Data.Image();
            request.Image.Content = bitmapString;

            //tell google that we want to perform label detection
            request.Features = new List<Google.Apis.Vision.v1.Data.Feature>();
            request.Features.Add(new Google.Apis.Vision.v1.Data.Feature() { Type = "LABEL_DETECTION" });
            var batch = new Google.Apis.Vision.v1.Data.BatchAnnotateImagesRequest();
            batch.Requests = new List<Google.Apis.Vision.v1.Data.AnnotateImageRequest>();
            batch.Requests.Add(request);

            //send request.  Note that I'm calling execute() here, but you might want to use
            //ExecuteAsync instead
            var apiResult = client.Images.Annotate(batch).Execute();

            // Dispose of the Java side bitmap.
            System.GC.Collect();
        }


        //    private void done(object sender, System.EventArgs e)
        //    {

        //        ////Make image available in the gallery
        //        //Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
        //        //var contentUri = Android.Net.Uri.FromFile(_file);
        //        //mediaScanIntent.SetData(contentUri);
        //        //SendBroadcast(mediaScanIntent);

        //        System.IO.FileStream fs = new System.IO.FileStream(_file.Path, System.IO.FileMode.OpenOrCreate);
        //        copy_bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 85, fs);
        //        fs.Flush();
        //        fs.Close();

        //        Finish();
        //    }

        //    private void removeRed(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);

        //        //checks if the button is checked
        //        if (box_checked[0] == false)
        //        {
        //            box_checked[0] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);
        //                    c.R = 0;                       
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //undoes all other color effects
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for(int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }
        //            //grabs the original bitmap and sets it as the image
        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);                
        //        }
        //    }

        //    private void removeBlue(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);

        //        if (box_checked[1] == false)
        //        {
        //            box_checked[1] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);
        //                    c.B = 0;
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }
        //        }
        //       else
        //        {
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }

        //    private void removeGreen(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);

        //        if (box_checked[2] == false)
        //        {
        //            box_checked[2] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);
        //                    c.G = 0;
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }

        //    private void negateRed(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);
        //        if (box_checked[3] == false)
        //        {
        //            box_checked[3] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);
        //                    c.R = Convert.ToByte(255 - c.R);
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }
        //    private void negateBlue(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);
        //        if (box_checked[4] == false)
        //        {
        //            box_checked[4] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);
        //                    c.B = Convert.ToByte(255 - c.B);
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }              
        //        }
        //        else
        //        {
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }
        //    private void negateGreen(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);
        //        if (box_checked[5] == false)
        //        {
        //            box_checked[5] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);
        //                    c.G = Convert.ToByte(255 - c.G);
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }               
        //        }
        //        else
        //        {
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }

        //    private void grayScale(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);
        //        if (box_checked[6] == false)
        //        {
        //            box_checked[6] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);

        //                    c.R = Convert.ToByte((c.R + c.G + c.B) / 3);
        //                    c.B = Convert.ToByte((c.R + c.G + c.B) / 3);
        //                    c.G = Convert.ToByte((c.R + c.G + c.B) / 3);
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }

        //    private void highContrast(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);
        //        if (box_checked[7] == false)
        //        {
        //            box_checked[7] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);

        //                    if(c.R > 150)
        //                    {
        //                        c.R = 255;
        //                    }
        //                    else
        //                    {
        //                        c.R = 0;
        //                    }
        //                    if (c.B > 150)
        //                    {
        //                        c.B = 255;
        //                    }
        //                    else
        //                    {
        //                        c.B = 0;
        //                    }
        //                    if (c.G > 150)
        //                    {
        //                        c.G = 255;
        //                    }
        //                    else
        //                    {
        //                        c.G = 0;
        //                    }

        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.addNoise);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }

        //    private void addNoise(object sender, System.EventArgs e)
        //    {
        //        CheckBox effect = sender as CheckBox;
        //        ImageView editView = FindViewById<ImageView>(Resource.Id.editImage);
        //        if (box_checked[8] == false)
        //        {
        //            box_checked[8] = true;
        //            for (int i = 0; i < copy_bitmap.Width; i++)
        //            {
        //                for (int j = 0; j < copy_bitmap.Height; j++)
        //                {
        //                    int p = copy_bitmap.GetPixel(i, j);
        //                    Color c = new Color(p);
        //                    Random rand_num = new Random();
        //                    int add_to_color = rand_num.Next(100);
        //                    int r = add_to_color + c.R;
        //                    if(r > 255)
        //                    {
        //                        r = 255;
        //                    }
        //                    int b = add_to_color + c.B;
        //                    if (b > 255)
        //                    {
        //                        b = 255;
        //                    }
        //                    int g = add_to_color + c.G;
        //                    if (g > 255)
        //                    {
        //                        g = 255;
        //                    }

        //                    c.R = Convert.ToByte(r);
        //                    c.B = Convert.ToByte(b);
        //                    c.G = Convert.ToByte(g);

        //                    if (c.R > 255)
        //                    {
        //                        c.R = 255;
        //                    }
        //                    else if(c.R < 0)
        //                    {
        //                        c.R = 0;
        //                    }
        //                    if (c.B > 255)
        //                    {
        //                        c.B = 255;
        //                    }
        //                    else if (c.B < 0)
        //                    {
        //                        c.B = 0;
        //                    }
        //                    if (c.G > 255)
        //                    {
        //                        c.G = 255;
        //                    }
        //                    else if (c.G < 0)
        //                    {
        //                        c.G = 0;
        //                    }
        //                    copy_bitmap.SetPixel(i, j, c);
        //                }
        //            }
        //        }
        //        else
        //        {

        //            CheckBox bttn0 = FindViewById<CheckBox>(Resource.Id.remBlue);
        //            bttn0.Checked = false;
        //            CheckBox bttn1 = FindViewById<CheckBox>(Resource.Id.remGreen);
        //            bttn1.Checked = false;
        //            CheckBox bttn2 = FindViewById<CheckBox>(Resource.Id.negRed);
        //            bttn2.Checked = false;
        //            CheckBox bttn3 = FindViewById<CheckBox>(Resource.Id.negBlue);
        //            bttn3.Checked = false;
        //            CheckBox bttn4 = FindViewById<CheckBox>(Resource.Id.negGreen);
        //            bttn4.Checked = false;
        //            CheckBox bttn5 = FindViewById<CheckBox>(Resource.Id.grayScale);
        //            bttn5.Checked = false;
        //            CheckBox bttn6 = FindViewById<CheckBox>(Resource.Id.highContrast);
        //            bttn6.Checked = false;
        //            CheckBox bttn7 = FindViewById<CheckBox>(Resource.Id.remRed);
        //            bttn7.Checked = false;

        //            for (int i = 0; i < box_checked.Length; i++)
        //            {
        //                box_checked[i] = false;
        //            }

        //            copy_bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, 1024, 768, true);
        //            editView.SetImageBitmap(copy_bitmap);
        //        }
        //    }

        //}
    }
}

