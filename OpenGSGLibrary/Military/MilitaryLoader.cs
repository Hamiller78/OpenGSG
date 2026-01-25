using OpenGSGLibrary.GameFilesParser;
using OpenGSGLibrary.Tools;

namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Loads country military definitions from history/units/*.txt files.
    /// </summary>
    public static class MilitaryLoader
    {
        /// <summary>
        /// Loads all military definitions from the units history folder.
        /// </summary>
        public static Dictionary<string, NationMilitary> LoadMilitaries(string unitsPath)
        {
            var militaries = new Dictionary<string, NationMilitary>();

            if (!Directory.Exists(unitsPath))
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(
                        LogLevel.Warning,
                        $"Military units directory not found: {unitsPath}"
                    );
                return militaries;
            }

            var unitFiles = Directory.GetFiles(unitsPath, "*.txt");
            GlobalLogger
                .GetInstance()
                .WriteLine(LogLevel.Info, $"Loading {unitFiles.Length} country military file(s)");

            foreach (var file in unitFiles)
            {
                try
                {
                    using var rawFile = File.OpenText(file);

                    var scanner = new Scanner();
                    var parser = new Parser();

                    var tokenStream = scanner.Scan(rawFile);
                    var parsedData = parser.Parse(tokenStream);

                    // Get country tag from file
                    var tag = GetString(parsedData, "tag");
                    if (string.IsNullOrEmpty(tag))
                    {
                        GlobalLogger
                            .GetInstance()
                            .WriteLine(LogLevel.Warning, $"No tag found in {file}");
                        continue;
                    }

                    var military = new NationMilitary();
                    military.SetData(tag, parsedData);
                    militaries[tag] = military;

                    GlobalLogger
                        .GetInstance()
                        .WriteLine(
                            LogLevel.Info,
                            $"Loaded military for {tag}: {military.Armies.Count} armies, {military.AirForces.Count} air forces"
                        );
                }
                catch (Exception ex)
                {
                    GlobalLogger
                        .GetInstance()
                        .WriteLine(
                            LogLevel.Err,
                            $"Error loading military file {file}: {ex.Message}"
                        );
                }
            }

            return militaries;
        }

        private static string GetString(
            ILookup<string, object> data,
            string key,
            string defaultValue = ""
        )
        {
            if (!data.Contains(key))
                return defaultValue;
            return data[key].FirstOrDefault()?.ToString() ?? defaultValue;
        }
    }
}
