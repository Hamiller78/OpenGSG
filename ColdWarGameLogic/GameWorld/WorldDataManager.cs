using System.IO;
using GameLogic;
using Map;
using Military;
using WorldData;

namespace WorldData
{
    /// <summary>
    /// Top class to handle all world game data (provinces, countries, units, etc...)
    /// </summary>
    public class WorldDataManager
    {
        // Preserve VB name to avoid breaking callers
        public ProvinceMap provinceMap { get; } = new ProvinceMap();

        /// <summary>
        /// Loads game data from game files.
        /// Should be called at start of program.
        /// </summary>
        /// <param name="gamedataPath">String with path to game files.</param>
        public void LoadAll(string gamedataPath)
        {
            if (!Directory.Exists(gamedataPath))
            {
                throw new DirectoryNotFoundException(
                    $"Given gamedata directory not found: {gamedataPath}"
                );
            }

            LoadWorldmap(Path.Combine(gamedataPath, "map"));
            provinceMap.LoadProvinceRGBs(Path.Combine(gamedataPath, "map", "definitions.csv"));
        }

        private void LoadWorldmap(string filePath)
        {
            provinceMap.FromFile(Path.Combine(filePath, "provinces.bmp"));
        }
    }
}
