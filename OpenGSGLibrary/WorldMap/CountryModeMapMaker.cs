using System.Collections.Generic;
using System.Drawing;
using WorldData;

namespace Map
{
    /// <summary>
    /// Creates an image where each province is coloured by its owner's country colour.
    /// </summary>
    public class CountryModeMapMaker : ModeMapMaker
    {
        private ProvinceMap provinceMap_;

        public CountryModeMapMaker(ProvinceMap provinceMap)
            : base(provinceMap)
        {
            provinceMap_ = provinceMap;
        }

        /// <summary>
        /// Generates a country-coloured map image from the given world state.
        /// </summary>
        public override Image MakeMap(WorldState sourceState)
        {
            var mapSize = sourceMap_.sourceBitmap!.Size;
            var provinceMap = sourceState.GetProvinceTable();
            var countryMapDict = sourceState.GetCountryTable();

            var countryMap = new Bitmap(
                mapSize.Width,
                mapSize.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );
            for (int y = 0; y < mapSize.Height; y++)
            {
                for (int x = 0; x < mapSize.Width; x++)
                {
                    var provinceRgb = sourceMap_.GetPixelRgb(x, y);
                    var provinceId = provinceMap_.GetProvinceNumber(provinceRgb);
                    countryMap.SetPixel(x, y, GetCountryDrawColor(provinceId, sourceState));
                }
            }

            return countryMap;
        }

        private Color GetCountryDrawColor(int provinceId, WorldState sourceState)
        {
            var drawColor = Color.AntiqueWhite;
            if (provinceId != -1)
            {
                var countryTag = sourceState.GetProvinceTable()![provinceId].Owner;
                var country = sourceState.GetCountryTable()![countryTag];
                var countryColor = country.GetColor();
                drawColor = Color.FromArgb(
                    countryColor.Item1,
                    countryColor.Item2,
                    countryColor.Item3
                );
            }
            return drawColor;
        }
    }
}
