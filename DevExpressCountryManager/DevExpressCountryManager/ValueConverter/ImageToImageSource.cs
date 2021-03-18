using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevExpressCountryManager.ValueConverter
{
    public class ImageToImageSource : IValueConverter
    {
        public object Convert(object imageObj, Type targetType, Object par, CultureInfo ci)
        {
            Image image = imageObj as Image;
            if (image != null)
            {
                return CreateBitmapSourceFromGdiBitmap(image);
            }

            return null;
        }

        public object ConvertBack(object imageSourceObj, Type typ, object obj2, CultureInfo ci)
        {
            return null;
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            Bitmap bitmap = new Bitmap(image);
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

    }
}
