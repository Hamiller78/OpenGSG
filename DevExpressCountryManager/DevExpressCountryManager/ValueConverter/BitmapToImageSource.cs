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
    public class BitmapToImageSource : IValueConverter
    {
        public object Convert(object bitmapObj, Type targetType, Object par, CultureInfo ci)
        {
            Bitmap bitmap = bitmapObj as Bitmap;
            if (bitmap != null)
            {
                return CreateBitmapSourceFromGdiBitmap(bitmap);
            }

            return null;
        }

        public object ConvertBack(object imageSourceObj, Type typ, object obj2, CultureInfo ci)
        {
            return null;
        }

        public static BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

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
