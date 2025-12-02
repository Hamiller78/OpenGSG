using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace OpenGSGLibrary.GameDataManager
{
    /// <summary>
    /// Base class for countries. Handles the most basic country properties.
    /// </summary>
    public class Country : GameObject
    {
        // Bitmap for the country's flag. Nullable to satisfy nullable reference types.
        public Bitmap? Flag { get; set; } = null;

        private string tag_;
        private string name_;
        private Tuple<byte, byte, byte> color_;

        public string GetTag() => tag_;

        public string GetName() => name_;

        public Tuple<byte, byte, byte> GetColor() => color_;

        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);

            name_ = Path.GetFileNameWithoutExtension(fileName);

            // parsedData is expected to contain entries like "tag" and "color" similar to the original VB Lookup
            tag_ = parsedData["tag"].Single()?.ToString();
            var colorList = (List<int>)parsedData["color"].Single();
            byte rValue = (byte)colorList[0];
            byte gValue = (byte)colorList[1];
            byte bValue = (byte)colorList[2];
            color_ = new Tuple<byte, byte, byte>(rValue, gValue, bValue);
        }

        public virtual void LoadFlags(string flagPath)
        {
            var path = Path.Combine(flagPath, GetTag() + ".png");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Flag image not found: " + path);
            }

            // Load bitmap now that System.Drawing.Common is referenced by the project.
            var img = Image.FromFile(path);
            Flag = new Bitmap(img);
        }
    }
}
