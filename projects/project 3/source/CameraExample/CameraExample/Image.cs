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
using Android.Graphics;

namespace CameraExample
{
    public class Image
    {
        private string[] words = new string[] { "Black", "Tree", "Cup", "Phone", "Brown" };
        private int progress = 0;
        private int lvl = 0;
        public Bitmap bitmap;
        public List<string> tags = new List<string>();
        bool imageCheck = false;

        public void SubmitPic()
        {
            imageCheck = false;

            if (CheckBitmap() == true)
            {
                GetAPI();
            }

            for (int i = 0; i < GetTagsLength(); i++)
            {
                if (GetTags(i) == GetWords(word_track))
                {
                    imageCheck = true;
                }
            }

            UpdatePoints();
        }

        public void SetBitmap(Bitmap map)
        {
             bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(map, 1024, 768, true);
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }

        public string GetTags(int place)
        {
            return tags[place];
        }

        public int GetTagsLength()
        {
            return tags.Count;
        }
        
        public bool CheckBitmap()
        {
            if(bitmap != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateLvl()
        {
            if(progress <= 100)
            {
                lvl++;
            }
        }
        
        public void UpdatePoints()
        {
            if (imageCheck == true)
            {
                progress = progress + 20;
            }
            else if (imageCheck == false)
            {
                if (progress == 0 || progress < 20)
                {
                    progress = 0;
                }
                else
                {
                    progress = progress - 20;
                }
            }
        }

        public string GetWords(int spot)
        {
            if(spot > 5)
            {
                spot = 5;
            }

            return words[spot];
        }

        public void GetAPI()
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
            
            foreach (var item in apiResult.Responses[0].LabelAnnotations)
            {
                tags.Add(item.Description);
            }
            //string question = string.Format("is this (a(n)) {0}?", tags[0]);
            //TextView output = FindViewById<TextView>(Resource.Id.gameText);
            //output.Text = question;
        }


    }
}