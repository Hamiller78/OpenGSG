using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OpenGSGLibraryV2.Maps
{
    public class ProvinceBitmap
    {
        public BitmapImage SourceBitmap { get; private set; }
        public byte[] SourcePixelArray { get; private set; }

        private FormatConvertedBitmap _formattedBitmap = new FormatConvertedBitmap();

        public void FromFile(string filePath)
        {
            try
            {
                string fullPath = Path.GetFullPath(filePath);
                SourceBitmap = new BitmapImage(new System.Uri(fullPath));
                MakeRgb24FormattedBitmap();
                SourcePixelArray = BitmapImageToArray(_formattedBitmap);
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }
        }

        public Tuple<byte, byte, byte> GetPixelRgb(int x, int y)
        {
            int pixelIndex = GetRgb24ArrayIndex(x, y);
            byte red = SourcePixelArray[pixelIndex];
            byte green = SourcePixelArray[pixelIndex + 1];
            byte blue = SourcePixelArray[pixelIndex + 2];
            return new Tuple<byte, byte, byte>(red, green, blue);
        }

        private void MakeRgb24FormattedBitmap()
        {
            _formattedBitmap.BeginInit();
            _formattedBitmap.Source = SourceBitmap;
            _formattedBitmap.DestinationFormat = PixelFormats.Rgb24;
            _formattedBitmap.EndInit();
        }

        private byte[] BitmapImageToArray(BitmapSource bmImage)
        {
            int stride = bmImage.PixelWidth * (bmImage.Format.BitsPerPixel / 8);
            byte[] pixelArray = new byte[bmImage.PixelHeight * stride];

            bmImage.CopyPixels(pixelArray, stride, 0);

            return pixelArray;
        }

        private int GetRgb24ArrayIndex(int x, int y)
        {
            return 3 * (y * _formattedBitmap.PixelWidth + x);
        }

    }
}
