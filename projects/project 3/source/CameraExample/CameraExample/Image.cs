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
        public Bitmap bitmap;


        public void SetBitmap(Bitmap map)
        {
             bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(map, 1024, 768, true);
        }
        
        private void AddProgress(int add)
        {
            progress = progress + add;
        }

        private string GetWords(int spot)
        {
            if(spot > 5)
            {
                spot = 5;
            }

            return words[spot];
        }
        

    }
}