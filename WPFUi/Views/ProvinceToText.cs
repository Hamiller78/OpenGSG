using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using OpenGSGLibrary.WorldData;

namespace WPFUi.Views
{
    public class ProvinceToText : IValueConverter
    {

        public IDictionary<int, Province> ProvinceTable { private get; set; }

        public object Convert(object provinceIdObj, Type targetType, Object par, CultureInfo ci)
        {
            int provinceId = (int)provinceIdObj;
            if (ProvinceTable.ContainsKey(provinceId))
            {
                Province currentProvince = ProvinceTable[provinceId];
                return GetProvinceInfo(currentProvince);
            }
            else
            {
                return "No data\nfor province";
            }
        }

        public object ConvertBack(object obj, Type typ, object obj2, CultureInfo ci)
        {
            return null;
        }

        private string GetProvinceInfo(Province prov)
        {
            string infoText = prov.GetId().ToString().Trim() + ": " + prov.GetName() + "\n";
            infoText += "Owner: " + prov.GetOwner() + "\n";
            return infoText;
        }
    }
}
