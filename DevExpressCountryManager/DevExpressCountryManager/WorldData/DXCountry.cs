using System;
using System.Collections.Generic;
using System.Text;
using OpenGSGLibrary.WorldData;

namespace DevExpressCountryManager.WorldData
{
    public class DXCountry : Country
    {


        public override string ToString()
        {
            return GetName();
        }
    }
}
