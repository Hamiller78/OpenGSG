using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenGSGLibrary.Events;
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

        /// <summary>
        /// Gets the event manager containing all loaded events.
        /// </summary>
        public EventManager EventManager => _eventManager;

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
                _countryTable = GameObjectFactory.FromFolder<string, Country, TCountry>(
                    Path.Combine(gamedataPath, "common\\countries"),
                    "tag"
                );
            }
            catch (Exception)
            {
                GlobalLogger
                    .GetInstance()
                    .WriteLine(LogLevel.Fatal, "Error while loading country data.");
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
