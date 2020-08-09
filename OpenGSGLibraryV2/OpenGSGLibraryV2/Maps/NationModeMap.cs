//    OpenSGSGLibrary is an open-source library for Grand Strategy Games
//    Copyright (C) 2019-2020  Torben Kneesch
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using OpenGSGLibrary.WorldData;

namespace OpenGSGLibraryV2.Maps
{
    public class NationModeMap : IModeMap
    {
        public BitmapSource modeBitmap { get; private set; }

        private ProvinceBitmap _sourceMap;
        private OpenGSGLibrary.Map.ProvinceMap _provinceMapping;

        public NationModeMap(ProvinceBitmap sourceMap,
                             OpenGSGLibrary.Map.ProvinceMap provinceMapping)
        {
            _sourceMap = sourceMap;
            _provinceMapping = provinceMapping;
        }

        public void Update(IDictionary<int, String> provinceOwnerList, WorldState currentState)
        {
            byte[] sourceMapArray = _sourceMap.SourcePixelArray;
            byte[] nationMapArray = new byte[sourceMapArray.Length];

            IDictionary < String, Tuple<byte, byte, byte> > nationColorList = GetCountryColors(currentState);

            for (int i = 0; i < sourceMapArray.Length; i += 3)
            {
                Tuple<byte, byte, byte> provinceRgb =
                    new Tuple<byte, byte, byte>(sourceMapArray[i], sourceMapArray[i + 1], sourceMapArray[i + 2]);
                int provinceId = _provinceMapping.GetProvinceNumber(provinceRgb);

                Tuple<byte, byte, byte> nationColor = new Tuple<byte, byte, byte>(0, 0, 180);
                if (provinceId >= 0)
                {
                    string owningCountry;
                    if (provinceOwnerList.TryGetValue(provinceId, out owningCountry))
                    {
                        nationColorList.TryGetValue(owningCountry, out nationColor);
                    }
                }
                nationMapArray[i] = nationColor.Item1;
                nationMapArray[i + 1] = nationColor.Item2;
                nationMapArray[i + 2] = nationColor.Item3;
            }

            modeBitmap = BitmapSource.Create(_sourceMap.SourceBitmap.PixelWidth,
                                             _sourceMap.SourceBitmap.PixelHeight,
                                             _sourceMap.SourceBitmap.DpiX,
                                             _sourceMap.SourceBitmap.DpiY,
                                             PixelFormats.Rgb24,
                                             _sourceMap.SourceBitmap.Palette,
                                             nationMapArray,
                                             _sourceMap.SourceBitmap.PixelWidth * 3);
        }

        private IDictionary<String, Tuple<byte, byte, byte>> GetCountryColors(WorldState currentState)
        {
            IDictionary<string, Country> countryTable = currentState.GetCountryTable();
            IDictionary<String, Tuple<byte, byte, byte>> nationColorList = new Dictionary<String, Tuple<byte, byte, byte>>();

            foreach (KeyValuePair<string, Country> ctry in countryTable)
            {
                nationColorList.Add(ctry.Key, ctry.Value.GetColor());
            }

            return nationColorList;
        }

    }
}
