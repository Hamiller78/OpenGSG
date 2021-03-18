using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using OpenGSGLibrary.WorldData;
using DevExpressCountryManager.Models.WorldData;

namespace DevExpressCountryManager
{
    public class MainWindowViewModel
    {
        private const string GAMEDATA_PATH = @"..\..\..\..\..\ColdWarPrototype\GameData";

        public ObservableCollection<DXCountry> Countries { get; set; } = new ObservableCollection<DXCountry>();

        private WorldLoader<Province, DXCountry> _worldLoader = new WorldLoader<Province, DXCountry>();

        public MainWindowViewModel()
        {
            LoadCountries(GAMEDATA_PATH);
        }

        public void LoadCountries(string filepath)
        {
            try
            {
                WorldState loadedState = _worldLoader.CreateStartState(filepath);
                Dictionary<string, Country> countryDictionary = (Dictionary<string, Country>)loadedState.GetCountryTable();
                Countries.Clear();
                foreach (KeyValuePair<string, Country> itemPair in countryDictionary)
                {
                    Countries.Add((DXCountry)(itemPair.Value));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while loading data: " + ex.Message);
                // TODO: Do something about the error
            }
        }

        // Just a test
        public void ModifyGermanAllegiance(string newAllegiance)
        {
            DXCountry germany = Countries.Where(c => c.GetTag() == "FRG").FirstOrDefault();
            if (germany != null)
            {
                germany.Allegiance = newAllegiance;
            }
        }
    }
}