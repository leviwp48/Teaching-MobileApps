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

        public void SetTags(List<string> tag_list)
        {
            tags = tag_list;
        }
        public void SubmitPic(int word_track)
        {
            imageCheck = false;

            for (int i = 0; i < GetTagsLength(); i++)
            {
                if (GetTags(i) == GetWords(word_track))
                {
                    imageCheck = true;
                }
            }

            UpdatePoints();
        }

        public int GetPoints()
        {
            return progress;
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

            if (progress <= 100)
            {
                lvl++;
                progress = 0;
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

       


    }
}