using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameFilesParser;
using OpenGSGLibrary.Localization;
using OpenGSGLibrary.Military;
using OpenGSGLibrary.Tools;

namespace OpenGSGLibrary.GameDataManager
{
    /// <summary>
    /// Class to create a world state from gamedata files.
    /// The derived classes for provinces and countries have to be specified when creating an instance of the class.
    /// </summary>
    public class WorldLoader<TProv, TCountry>
        where TProv : Province, new()
        where TCountry : Country, new()
    {
        private IDictionary<int, Province> _provinceTable = default!;
        private IDictionary<string, Country> _countryTable = default!;
        private Dictionary<string, Unit> _unitDefinitions = new();
        private EventManager _eventManager = new();
        private readonly LocalizationManager _localizationManager = new();

        /// <summary>
        /// Gets the event manager containing all loaded events.
        /// </summary>
        public EventManager EventManager => _eventManager;

        /// <summary>
        /// Gets the localization manager containing all loaded translations.
        /// </summary>
        public LocalizationManager LocalizationManager => _localizationManager;

        /// <summary>
        /// Gets the unit definitions loaded from common/units.
        /// </summary>
        public Dictionary<string, Unit> UnitDefinitions => _unitDefinitions;

        /// <summary>
        /// Creates a world state from the data in the game data or mod directories.
        /// This should be the start state for a game.
        /// </summary>
        /// <param name="gamedataPath">Path to game data directory.</param>
        /// <returns>WorldState with loaded game data including events.</returns>
        public WorldState CreateStartState(string gamedataPath)
        {
            var newState = new WorldState();
            try
            {
                LoadLocalizations(gamedataPath);

                LoadProvinces(gamedataPath);

                LoadCountries(gamedataPath);
                LoadCountryFlags(gamedataPath);

                // Load unit definitions before militaries (militaries reference unit types)
                LoadUnitDefinitions(gamedataPath);

                // Load and attach militaries to countries
                LoadMilitaries(gamedataPath);

                LoadEvents(gamedataPath);

                newState.SetProvinceTable(_provinceTable);
                newState.SetCountryTable(_countryTable);

                _countryTable.Values.ToList().ForEach(c => c.UpdateProvinces(newState));

                return newState;
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(
                        LogLevel.Fatal,
                        "Could not load initial world state from: " + gamedataPath
                    );
                throw;
            }
        }

        private void LoadLocalizations(string gamedataPath)
        {
            try
            {
                var localizationPath = Path.Combine(gamedataPath, "localization");
                _localizationManager.LoadFromDirectory(localizationPath);
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading localization data.");
                throw;
            }
        }

        private void LoadProvinces(string gamedataPath)
        {
            try
            {
                _provinceTable = GameObjectFactory.FromFolderWithFilenameId<Province, TProv>(
                    Path.Combine(gamedataPath, "history", "provinces")
                );
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading province data.");
                throw;
            }
        }

        private void LoadCountries(string gamedataPath)
        {
            try
            {
                // Load basic country definitions from common/countries
                _countryTable = GameObjectFactory.FromFolder<string, Country, TCountry>(
                    Path.Combine(gamedataPath, "common", "countries"),
                    "tag"
                );

                // Load and merge historical data from history/countries
                LoadCountryHistoricalData(gamedataPath);
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading country data.");
                throw;
            }
        }

        private void LoadCountryHistoricalData(string gamedataPath)
        {
            try
            {
                var historyPath = Path.Combine(gamedataPath, "history", "countries");

                if (!Directory.Exists(historyPath))
                {
                    GlobalLogger
                        .GetInstance()
                        .WriteLine(
                            LogLevel.Warning,
                            $"Country history directory not found: {historyPath}"
                        );
                    return;
                }

                var historyFiles = Directory.GetFiles(historyPath, "*.txt");
                var loadedCount = 0;

                foreach (var file in historyFiles)
                {
                    try
                    {
                        // Extract tag from filename: "USA - United States.txt" -> "USA"
                        var filenameParts = GameObjectFactory.ExtractFromFilename(file);
                        if (filenameParts.Length == 0)
                            continue;

                        var tag = filenameParts[0].Trim();

                        if (_countryTable.TryGetValue(tag, out var country))
                        {
                            // Parse the historical data file
                            using var rawFile = File.OpenText(file);
                            var scanner = new Scanner();
                            var parser = new Parser();
                            var tokenStream = scanner.Scan(rawFile);
                            var parsedData = parser.Parse(tokenStream);

                            // Call SetData again to merge historical properties
                            country.SetData(file, parsedData);
                            loadedCount++;
                        }
                        else
                        {
                            GlobalLogger
                                .GetInstance()
                                .WriteLine(
                                    LogLevel.Warning,
                                    $"Historical data file {Path.GetFileName(file)} references unknown country tag: {tag}"
                                );
                        }
                    }
                    catch (Exception ex)
                    {
                        GlobalLogger
                            .GetInstance()
                            .WriteLine(
                                LogLevel.Err,
                                $"Error loading historical data from {Path.GetFileName(file)}: {ex.Message}"
                            );
                    }
                }

                GlobalLogger
                    .GetInstance()
                    .WriteLine(
                        LogLevel.Info,
                        $"Loaded historical data for {loadedCount} country/countries"
                    );
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading country history data.");
                throw;
            }
        }

        private void LoadCountryFlags(string gamedataPath)
        {
            try
            {
                var flagPath = Path.Combine(gamedataPath, "gfx", "flags");
                if (_countryTable != null)
                {
                    foreach (var country in _countryTable)
                    {
                        country.Value.LoadFlags(flagPath);
                    }
                }
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading nation flags.");
                throw;
            }
        }

        private void LoadUnitDefinitions(string gamedataPath)
        {
            try
            {
                var unitsPath = Path.Combine(gamedataPath, "common", "units");
                _unitDefinitions = UnitLoader.LoadUnits(unitsPath);

                GlobalLogger
                    .GetInstance()
                    .WriteLine(
                        LogLevel.Info,
                        $"Loaded {_unitDefinitions.Count} unit definition(s)"
                    );
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading unit definitions.");
                throw;
            }
        }

        private void LoadMilitaries(string gamedataPath)
        {
            try
            {
                var militaryHistoryPath = Path.Combine(gamedataPath, "history", "units");
                var militaries = MilitaryLoader.LoadMilitaries(militaryHistoryPath);

                // Attach militaries to countries and calculate strength
                var attachedCount = 0;
                foreach (var (tag, military) in militaries)
                {
                    if (_countryTable.TryGetValue(tag, out var country))
                    {
                        country.Military = military;

                        // Calculate and set military strength from units
                        var totalStrength = military.GetTotalStrength(_unitDefinitions);
                        country.MilitaryStrength = totalStrength;

                        attachedCount++;

                        GlobalLogger
                            .GetInstance()
                            .WriteLine(
                                LogLevel.Info,
                                $"Attached military to {tag}: {military.Armies.Count} armies, "
                                    + $"{military.AirForces.Count} air forces, strength={totalStrength}"
                            );
                    }
                    else
                    {
                        GlobalLogger
                            .GetInstance()
                            .WriteLine(
                                LogLevel.Warning,
                                $"Military data for unknown country: {tag}"
                            );
                    }
                }

                GlobalLogger
                    .GetInstance()
                    .WriteLine(
                        LogLevel.Info,
                        $"Attached militaries to {attachedCount} country/countries"
                    );
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading military data.");
                throw;
            }
        }

        private void LoadEvents(string gamedataPath)
        {
            try
            {
                var eventsPath = Path.Combine(gamedataPath, "events");
                _eventManager = EventFactory.LoadFromFolder(eventsPath);
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading event data.");
                throw;
            }
        }
    }
}
