using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CameraExample
{
    [Activity(Label = "VisionGame")]
    public class VisionGame : Activity
    {

        Android.Graphics.Bitmap bitmap; 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.VisionGame);

            ImageView gamePicture = FindViewById<ImageView>(Resource.Id.gameImage);

            if (gamePicture != null)
            {
                gamePicture.SetImageBitmap(bitmap);
            }

            Bundle extras = send_data.GetParcelableExtra("Image");
            bitmap = (bitmap)extras.GetParcelable("Image");

            //string question = string.Format("is this (a(n)) {0}?", tags[0]);
            //TextView output = FindViewById<TextView>(Resource.Id.gameText);
            //output.Text = question;

        }

        private void getImage(object sender, EventArgs e)
        {
            
        }
        protected void getAPI(object sender, EventArgs e)
        {
                       
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
            List<string> tags = new List<string>();
            foreach (var item in apiResult.Responses[0].LabelAnnotations)
            {
                tags.Add(item.Description);
            }
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