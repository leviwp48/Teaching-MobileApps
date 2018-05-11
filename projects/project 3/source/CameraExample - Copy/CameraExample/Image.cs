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
        private string[] words = new string[] { "circle", "black and white", "text", "blue", "font" };
        private int progress = 0;
        private int lvl = 0;
        public Bitmap bitmap;
        public List<string> tags = new List<string>();
        private bool done;

        public void SetTags(List<string> tag_list)
        {
            tags = tag_list;
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
    
        public bool GetDone()
        {
            return done;
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
        
        public int GetLvl()
        {
            return lvl;
        }

        public int GetProgress()
        {
            return progress;
        }

        public void UpdatePoints(bool imageCheck)
        {
            done = false; 

            if (imageCheck == true)
            {              
                progress = progress + 20;

                if (progress >= 100)
                {
                    done = true; 
                    lvl++;
                    progress = 0;
                }
            }
            else 
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

       


    }
}