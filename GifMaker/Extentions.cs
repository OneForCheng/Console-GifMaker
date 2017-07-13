using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;


namespace GifMaker
{
    public static class Extentions
    {
        public static Bitmap GetBitmap(this BitmapSource source)
        {
            //判断source.Format是否是PixelFormats.Bgra32没有什么用的，必须转换一次才能保证
            var formatBitmap = new FormatConvertedBitmap();
            formatBitmap.BeginInit();
            formatBitmap.Source = source;
            formatBitmap.DestinationFormat = PixelFormats.Bgra32;
            formatBitmap.EndInit();

            var bitmap = new Bitmap(formatBitmap.PixelWidth, formatBitmap.PixelHeight, PixelFormat.Format32bppArgb);
            var data = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            formatBitmap.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bitmap.UnlockBits(data);

            return bitmap;
        }

        public static BitmapImage GetBitmapImage(string filePath)
        {
            var stream = new MemoryStream();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(stream);
            }
            stream.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static Bitmap ResizeBitmap(this Bitmap bitmap, int width, int height)
        {
            if (bitmap.Width == width && bitmap.Height == height)
            {
                return (Bitmap)bitmap.Clone();
            }
            var resizeBitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (var bmpGraphics = Graphics.FromImage(resizeBitmap))
            {
                bmpGraphics.SmoothingMode = SmoothingMode.HighQuality;
                bmpGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                bmpGraphics.CompositingQuality = CompositingQuality.GammaCorrected;
                bmpGraphics.DrawImage(bitmap, 0, 0, width, height);
            }
            return resizeBitmap;
        }

    }
}
