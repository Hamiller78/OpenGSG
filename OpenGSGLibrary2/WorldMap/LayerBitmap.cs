using System;
using System.Drawing;

namespace Map
{
    /// <summary>
    /// Base class for bitmaps of world map.
    /// </summary>
    public class LayerBitmap
    {
        public Bitmap? sourceBitmap { get; private set; }

        /// <summary>
        /// Sets the contained bitmap from a file.
        /// </summary>
        /// <param name="filePathAndName">String with full file path of image file.</param>
        public void FromFile(string filePathAndName)
        {
            var img = Image.FromFile(filePathAndName);
            sourceBitmap = new Bitmap(img);
        }

        /// <summary>
        /// Gets the RGB value of a pixel in the bitmap.
        /// </summary>
        /// <param name="x">x coordinate of target pixel in original image coordinates.</param>
        /// <param name="y">y coordinate of target pixel in original image coordinates.</param>
        /// <returns>RGB value as tuple of 3 bytes.</returns>
        public Tuple<byte, byte, byte> GetPixelRgb(int x, int y)
        {
            try
            {
                if (sourceBitmap == null) return Tuple.Create((byte)0, (byte)0, (byte)0);
                var color = sourceBitmap.GetPixel(x, y);
                return Tuple.Create(color.R, color.G, color.B);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Tuple.Create((byte)0, (byte)0, (byte)0);
            }
        }
    }
}
