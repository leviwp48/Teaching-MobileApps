 using System;
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
        int _word_track = 0;
        bool imageCheck = false;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.GameView);

            // TODO: Make text change when progress bar fills up.
            TextView wordToFind = FindViewById<TextView>(Resource.Id.gameText);
            //make method to grab word
            wordToFind.Text = string.Format("Category: {0}", (image.GetWords(WordTrack)));
            //string question = string.Format("is this (a(n)) {0}?", tags[0]);
            //TextView output = FindViewById<TextView>(Resource.Id.gameText);
            //output.Text = question;
       
            Button cam = FindViewById<Button>(Resource.Id.takePhoto);
            cam.Click += TakePicture;

            Button submit = FindViewById<Button>(Resource.Id.btn_submit);
            submit.Click += SubmitPic;

            if (image.GetLvl() == 5)
            {
                SetContentView(Resource.Layout.Finish);

                ImageView last_image = FindViewById<ImageView>(Resource.Id.last_pic);
                TextView last_word = FindViewById<TextView>(Resource.Id.last_text);

                last_word.Text = string.Format("This is a: {0}", (image.GetWords(0)));
                last_image.SetImageBitmap(image.GetBitmap());
            }
        }

        public void wordChange()
        {
            // TODO: Make text change when progress bar fills up.
            TextView wordToFind = FindViewById<TextView>(Resource.Id.gameText);
            //make method to grab word
            wordToFind.Text = string.Format("Category: {0}", (image.GetWords(WordTrack)));
        }
        public int WordTrack
        {
            get { return _word_track; }
            set
            {
                _word_track = value;
                if (_word_track == 1)
                {
                    wordChange();
                }
            }
        }

        private void SubmitPic(object sender, System.EventArgs e)
        {
            TextView wordToFind = FindViewById<TextView>(Resource.Id.resultsText);

            imageCheck = false;

            for (int i = 0; i < image.GetTagsLength(); i++)
            {
                if (image.GetTags(i) == image.GetWords(_word_track))
                {
                    imageCheck = true;
                }
            }
            image.UpdatePoints(imageCheck);

            ProgressBar bar = FindViewById<ProgressBar>(Resource.Id.progressBar1);          
            bar.Progress = image.GetPoints();

            ProgressBar lvlBar = FindViewById<ProgressBar>(Resource.Id.doneBar);           
            lvlBar.Progress = image.GetLvl();

            if (image.GetDone() == true)
            {
                WordTrack++;
            }

            if (imageCheck == false)
            {
                wordToFind.Text = ("Fail");
            }
            else
            {
                wordToFind.Text = ("Success");
            }
            
            if(image.GetLvl() == 5)
            {
                SetContentView(Resource.Layout.Finish);

                Button last_btn = FindViewById<Button>(Resource.Id.btn_last);
                TextView last_word = FindViewById<TextView>(Resource.Id.last_text);

                last_word.Text = string.Format("Category: {0}", (image.GetWords(WordTrack)));
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

        // Intent to take a picture
        private void TakePicture(object sender, System.EventArgs e)
        {           
            if (IsThereAnAppToTakePictures() == true)
            {
                Intent intent = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(intent, 0);
            }
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

            if (image.CheckBitmap() != false)
            {
                takenPic.SetImageBitmap(image.GetBitmap());
            }
            
            //convert bitmap into stream to be sent to Google API
            string bitmapString = "";
            using (var stream = new System.IO.MemoryStream())
            {
                image.GetBitmap().Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 0, stream);

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
            List<string> details = new List<string>();

            foreach (var item in apiResult.Responses[0].LabelAnnotations)
            {
                details.Add(item.Description);
            }

            image.SetTags(details);
            
           

            // Dispose of the Java side bitmap.
            System.GC.Collect();

            
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