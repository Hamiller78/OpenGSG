using System;
using System.Collections.Generic;
using System.IO;

namespace WorldData
{
    /// <summary>
    /// Class to create a world state from gamedata files.
    /// The derived classes for provinces, countries, etc. have to be specified when creating an instance of the class.
    /// </summary>
    public class WorldLoader<TProv, TCountry>
        where TProv : Province, new()
        where TCountry : Country, new()
    {
        public WorldState CreateStartState(string gamedataPath)
        {
            var newState = new WorldState();
            try
            {
                LoadProvinces(gamedataPath);
                LoadCountries(gamedataPath);
                LoadCountryFlags(gamedataPath);
                LoadArmies(gamedataPath);

                newState.SetProvinceTable(provinceTable_);
                newState.SetCountryTable(countryTable_);
                newState.SetArmyManager(armyManager_);

                return newState;
            }
            catch (Exception ex)
            {
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Could not load initial world state from: " + gamedataPath);
                throw;
            }
        }

        private void LoadProvinces(string gamedataPath)
        {
            try
            {
                provinceTable_ = GameObjectFactory.FromFolderWithFilenameId<Province, TProv>(Path.Combine(gamedataPath, "history\\provinces"));
            }
            catch (Exception)
            {
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading province data.");
                throw;
            }
        }

        private void LoadCountries(string gamedataPath)
        {
            try
            {
                countryTable_ = GameObjectFactory.FromFolder<string, Country, TCountry>(Path.Combine(gamedataPath, "common\\countries"), "tag");
            }
            catch (Exception)
            {
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading country data.");
                throw;
            }
        }

        private void LoadCountryFlags(string gamedataPath)
        {
            try
            {
                var flagPath = Path.Combine(gamedataPath, "gfx\\flags");
                if (countryTable_ != null)
                {
                    foreach (var country in countryTable_)
                    {
                        country.Value.LoadFlags(flagPath);
                    }
                }
            }
            catch (Exception)
            {
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading nation flags.");
                throw;
            }
        }

        private void LoadArmies(string gamedataPath)
        {
            try
            {
                armyManager_.LoadFolder(Path.Combine(gamedataPath, "history\\units"));
            }
            catch (Exception)
            {
                Tools.GlobalLogger.GetInstance().WriteLine(Tools.LogLevel.Fatal, "Error while loading army data.");
                throw;
            }
        }

        private IDictionary<int, Province>? provinceTable_ = null;
        private IDictionary<string, Country>? countryTable_ = null;
        private Military.ArmyManager armyManager_ = new Military.ArmyManager();
    }
}
