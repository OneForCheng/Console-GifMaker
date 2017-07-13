using System;
using System.IO;
using System.Linq;
using GifMaker.Encoding;

namespace GifMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new GifConfig();
            var bitmapFrames = config.BitmapFrames;
            if (bitmapFrames.Count > 2)
            {
                int width, height;
                if (config.IsSetSize)
                {
                    width = config.Width;
                    height = config.Height;
                }
                else
                {
                    width =  (int)bitmapFrames.Max(m => m.Width);
                    height = (int)bitmapFrames.Max(m => m.Height);
                }
              
                using (var stream = new MemoryStream())
                {
                    using (var encoder = new GifEncoder(stream, width, height, config.RepeatCount))
                    {
                        var delays = config.Delays;
                        var n = delays.Count;
                        var isLockAspect = config.IsLockAspect;
                        for (var i = 0; i < bitmapFrames.Count; i++)
                        {
                            using (var bitmap = bitmapFrames[i].GetBitmap())
                            {
                                var newWidth = bitmap.Width;
                                var newHeight = bitmap.Height;
                                if (!isLockAspect)
                                {
                                    newWidth = width;
                                    newHeight = height;
                                }
                                else
                                {
                                    try
                                    {
                                        if (newWidth > width)
                                        {
                                            newHeight = newHeight*width/newWidth;
                                            newWidth = width;
                                        }

                                        if (newHeight > height)
                                        {
                                            newWidth = newWidth*height/newHeight;
                                            newHeight = height;
                                        }
                                    }
                                    catch
                                    {
                                        //
                                    }
                                
                                }
                                if (newWidth <= 0)
                                {
                                    newWidth = 1;
                                }
                                if (newHeight <= 0)
                                {
                                    newHeight = 1;
                                }
                                using (var resizeBitmap = bitmap.ResizeBitmap(newWidth, newHeight))
                                {
                                    encoder.AppendFrame(resizeBitmap, delays[i < n ? i : n - 1]);
                                }
                            }
                        }
                    }
                    stream.Position = 0;
                    using (var file = File.Create(Guid.NewGuid().ToString("N") + ".gif"))
                    {   
                        stream.CopyTo(file);
                    }
                }
            }
            else
            {
                Console.WriteLine("至少需要2张原图才能生成Gif动图。");
            }
        }
    }
}
