using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using OpenGSGLibrary.WorldData;
using DevExpressCountryManager.Models.Common;

namespace DevExpressCountryManager.Models.WorldData
{
    public class DXCountry : Country
    {
        [Key]
        [StringLength(6)]
        public string Tag
        {
            get
            {
                return base.GetTag();
            }
            set
            {
                base.tag_ = value;
            }
        }

        public string LongName { get; set; }
        public string Government { get; set; }
        public string Allegiance { get; set; }
        public string Leader { get; set; }

        public int FlagId { get; set; }
        public virtual BlobbableImage Flag { get; set; }

        [NotMapped]
        public new Image flag
        {
            get
            {
                return Flag.ImageObj;
            }
            set
            {
                if (Flag == null)
                {
                    Flag = new BlobbableImage();
                }
                Flag.ImageObj = value;
            }
        }

        public DXCountry() : base()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        public override void SetData(string fileName, Lookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);

            LongName = parsedData["long_name"].Single() as string;
            Government = parsedData["government"].Single() as string;
            if (parsedData.Contains("allegiance"))
            {
                Allegiance = parsedData["allegiance"].Single() as string;
            }
            else
            {
                Allegiance = null;
            }
            Leader = parsedData["leader"].Single() as string;
        }

        public override void LoadFlags(string flagPath)
        {
            Image flagImage = Image.FromFile(Path.Combine(flagPath, GetTag() + ".png"));
            flag = flagImage;
        }

    }
}
