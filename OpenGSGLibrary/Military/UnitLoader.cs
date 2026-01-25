using OpenGSGLibrary.GameFilesParser;
using OpenGSGLibrary.Tools;

namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Loads unit type definitions from common/units/*.txt files.
    /// </summary>
    public static class UnitLoader
    {
        /// <summary>
        /// Loads all unit definitions from the units folder.
        /// </summary>
        public static Dictionary<string, Unit> LoadUnits(string unitsPath)
        {
            var units = new Dictionary<string, Unit>();

            if (!Directory.Exists(unitsPath))
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Warning, $"Units directory not found: {unitsPath}");
                return units;
            }

            var unitFiles = Directory.GetFiles(unitsPath, "*.txt");
            GlobalLogger
                .GetInstance()
                .WriteLine(LogLevel.Info, $"Loading {unitFiles.Length} unit definition(s)");

            foreach (var file in unitFiles)
            {
                try
                {
                    var typeId = Path.GetFileNameWithoutExtension(file);
                    using var rawFile = File.OpenText(file);

                    var scanner = new Scanner();
                    var parser = new Parser();

                    var tokenStream = scanner.Scan(rawFile);
                    var parsedData = parser.Parse(tokenStream);

                    // Unit files have a flat structure (no top-level key)
                    var unit = new Unit();
                    unit.SetData(typeId, parsedData);
                    units[typeId] = unit;

                    GlobalLogger
                        .GetInstance()
                        .WriteLine(
                            LogLevel.Info,
                            $"Loaded unit: {typeId} (type={unit.Type}, str={unit.Strength})"
                        );
                }
                catch (Exception ex)
                {
                    GlobalLogger
                        .GetInstance()
                        .WriteLine(LogLevel.Err, $"Error loading unit file {file}: {ex.Message}");
                }
            }

            return units;
        }
    }
}
