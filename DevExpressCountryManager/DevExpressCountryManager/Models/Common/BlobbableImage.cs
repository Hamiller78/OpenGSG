using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Text;

namespace DevExpressCountryManager.Models.Common
{
    public class BlobbableImage
    {
        public int BlobbableImageID { get; set; }
        public byte[] ImageData { get; set; }

        [NotMapped]
        public Image ImageObj
        {
            get { return ByteArrayToImage(ImageData); }
            set { ImageData = ImageToByteArray(value); }
        }

        private Image ByteArrayToImage(byte[] imageData)
        {
            MemoryStream ms = new MemoryStream(imageData);
            Image image = Image.FromStream(ms);
            return image;
        }

        private byte[] ImageToByteArray(Image imageObj)
        {
            MemoryStream ms = new MemoryStream();
            imageObj.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageArray = ms.ToArray();
            ms.Close();
            ms.Dispose();

            return imageArray;
        }
    }
}
