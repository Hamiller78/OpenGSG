using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

namespace OpenGSGLibrary.WorldMap
{
    /// <summary>
    /// Bitmap for province view. RGB values mark provinces, which are translated names and IDs in extra CSV file.
    /// </summary>
    public class ProvinceMap : LayerBitmap
    {
        private Dictionary<Tuple<byte, byte, byte>, int> _rgbProvinceMap = [];
        private Dictionary<int, string> _provinceNames = [];

        /// <summary>
        /// Gets the province number for a RGB value.
        /// </summary>
        /// <param name="rgbKey">RGB value as a tuple of three bytes.</param>
        /// <returns>Province number as integer, -1 if not found.</returns>
        public int GetProvinceNumber(Tuple<byte, byte, byte> rgbKey)
        {
            if (_rgbProvinceMap.ContainsKey(rgbKey))
                return _rgbProvinceMap[rgbKey];
            return -1;
        }

        /// <summary>
        /// Gets the province name for a RGB value.
        /// </summary>
        /// <param name="provinceNumber">Id number of province.</param>
        /// <returns>Province name as string, empty string if not found.</returns>
        public string GetProvinceName(int provinceNumber)
        {
            if (_provinceNames.TryGetValue(provinceNumber, out var name))
                return name;
            return string.Empty;
        }

        /// <summary>
        /// Loads province RGB values from a CSV file.
        /// Expected Format: Number;R;G;B;Name
        /// Ignores first line (headline).
        /// </summary>
        /// <param name="fullFilePath">Full file path of CSV file.</param>
        public void LoadProvinceRGBs(string fullFilePath)
        {
            using var csvParser = new TextFieldParser(fullFilePath);
            csvParser.TextFieldType = FieldType.Delimited;
            csvParser.SetDelimiters(";");

            var newRgbProvinceMap = new Dictionary<Tuple<byte, byte, byte>, int>();
            var newNameMap = new Dictionary<int, string>();
            var headline = csvParser.ReadFields(); // ignoring the headline

            while (!csvParser.EndOfData)
            {
                try
                {
                    var currentRow = csvParser.ReadFields();
                    var provinceColor = Tuple.Create(
                        (byte)Convert.ToInt32(currentRow[1]),
                        (byte)Convert.ToInt32(currentRow[2]),
                        (byte)Convert.ToInt32(currentRow[3])
                    );
                    var provinceNumber = Convert.ToInt32(currentRow[0]);
                    var provinceName = currentRow[4];
                    if (!newRgbProvinceMap.ContainsKey(provinceColor))
                    {
                        newRgbProvinceMap.Add(provinceColor, provinceNumber);
                        newNameMap.Add(provinceNumber, provinceName);
                    }
                }
                catch (MalformedLineException)
                {
                    Console.Write("Line couldn't be parsed");
                }
                catch (IndexOutOfRangeException)
                {
                    Console.Write("Line didn't contain enough fields");
                }
                catch (InvalidCastException)
                {
                    Console.Write("Cast error for one or more fields");
                }
            }

            _rgbProvinceMap = newRgbProvinceMap;
            _provinceNames = newNameMap;
        }
    }
}
