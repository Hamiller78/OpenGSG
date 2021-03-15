using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenGSGLibrary.WorldData;

namespace DevExpressCountryManager.WorldData
{
    public class DXCountry : Country
    {
        public string LongName { get; set; }
        public string Government { get; set; }
        public string Allegiance { get; set; }
        public string Leader { get; set; }

        public DXCountry() : base()
        {
        }

        public string Name
        {
            get
            {
                return GetName();
            }
        }

        public override string ToString()
        {
            return GetName();
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
    }
}
