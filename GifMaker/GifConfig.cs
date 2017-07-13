using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace GifMaker
{
    class GifConfig
    {
        public GifConfig()
        {
            int repeatCount;
            bool isLockAspect;
            string[] strs;
            RepeatCount = int.TryParse(ConfigurationManager.AppSettings["RepeatCount"], out repeatCount) ? repeatCount : 0;
            if (RepeatCount < 0)
            {
                RepeatCount = 0;
            }
           
            var str = ConfigurationManager.AppSettings["Size"];
            IsSetSize = false;

            if (!string.IsNullOrWhiteSpace(str))
            {
                strs = str.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length == 2)
                {
                    int width, height;
                    if (int.TryParse(strs[0], out width) && int.TryParse(strs[1], out height))
                    {
                        if (width > 0 && height > 0)
                        {
                            IsSetSize = true;
                            Width = width;
                            Height = height;
                        }
                    }
                }
            }

            IsLockAspect = !bool.TryParse(ConfigurationManager.AppSettings["IsLockAspect"], out isLockAspect) || isLockAspect;

            BitmapFrames = new List<BitmapSource>();
            str = ConfigurationManager.AppSettings["Images"];
            if (!string.IsNullOrWhiteSpace(str))
            {
                strs = str.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strs)
                {
                    if (File.Exists(item))
                    {
                        try
                        {
                            BitmapFrames.Add(Extentions.GetBitmapImage(item));
                        }
                        catch
                        {
                            //..
                        }
                    }
                }
            }

            Delays = new List<int>();
            str = ConfigurationManager.AppSettings["Delays"];
            if (!string.IsNullOrWhiteSpace(str))
            {
                strs = str.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in strs)
                {
                    int delay;
                    if (int.TryParse(item, out delay))
                    {
                        if (delay < 0)
                        {
                            delay = 500;
                        }
                    }
                    else
                    {
                        delay = 500;
                    }
                    Delays.Add(delay);
                }
            }
            if (Delays.Count == 0)
            {
                Delays.Add(500);
            }
        }

        public List<int> Delays { get; }

        public int RepeatCount { get; }

        public bool IsSetSize { get; }

        public int Width { get; }

        public int Height { get; }

        public bool IsLockAspect { get; }

        public List<BitmapSource> BitmapFrames { get; }
    }
}
