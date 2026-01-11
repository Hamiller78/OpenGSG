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
    /// The derived classes for provinces, countries, events, etc. have to be specified when creating an instance of the class.
    /// </summary>
    public class WorldLoader<TProv, TCountry, TCountryEvent, TNewsEvent>
        where TProv : Province, new()
        where TCountry : Country, new()
        where TCountryEvent : CountryEvent, new()
        where TNewsEvent : NewsEvent, new()
    {
        private IDictionary<int, Province> _provinceTable = default!;
        private IDictionary<string, Country> _countryTable = default!;
        private readonly ArmyManager _armyManager = new();
        private EventManager _eventManager = new();
        private LocalizationManager _localizationManager = new();

        /// <summary>
        /// Gets the event manager containing all loaded events.
        /// </summary>
        public EventManager EventManager => _eventManager;

        /// <summary>
        /// Gets the localization manager containing all loaded translations.
        /// </summary>
        public LocalizationManager LocalizationManager => _localizationManager;

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

                LoadArmies(gamedataPath);

                LoadEvents(gamedataPath);

                newState.SetProvinceTable(_provinceTable);
                newState.SetCountryTable(_countryTable);
                newState.SetArmyManager(_armyManager);

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
                    Path.Combine(gamedataPath, "history\\provinces")
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
                    Path.Combine(gamedataPath, "common\\countries"),
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
                var historyPath = Path.Combine(gamedataPath, "history\\countries");

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
                var flagPath = Path.Combine(gamedataPath, "gfx\\flags");
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

        private void LoadArmies(string gamedataPath)
        {
            try
            {
                _armyManager.LoadFolder(Path.Combine(gamedataPath, "history\\units"));
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading army data.");
                throw;
            }
        }

        private void LoadEvents(string gamedataPath)
        {
            try
            {
                var eventsPath = Path.Combine(gamedataPath, "events");
                _eventManager = EventFactory.LoadFromFolder<TCountryEvent, TNewsEvent>(eventsPath);
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
