using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using OpenGSGLibrary.WorldData;
using DevExpressCountryManager.Models.WorldData;
using DevExpressCountryManager.Database;
using DevExpressCountryManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DevExpressCountryManager
{
    public class MainWindowViewModel
    {
        private const string GAMEDATA_PATH = @"..\..\..\..\..\ColdWarPrototype\GameData";

        public ObservableCollection<DXCountryViewModel> Countries { get; set; } = new ObservableCollection<DXCountryViewModel>();

        private WorldLoader<Province, DXCountry> _worldLoader = new WorldLoader<Province, DXCountry>();

        public MainWindowViewModel()
        {
            // LoadCountriesFromFiles(GAMEDATA_PATH);
            CountryContext dbContext = new CountryContext();
            LoadCountriesFromDb(dbContext);
        }

        public void LoadCountriesFromFiles(string filepath)
        {
            try
            {
                WorldState loadedState = _worldLoader.CreateStartState(filepath);
                Dictionary<string, Country> countryDictionary = (Dictionary<string, Country>)loadedState.GetCountryTable();
                Countries.Clear();
                foreach (KeyValuePair<string, Country> itemPair in countryDictionary)
                {
                    DXCountryViewModel countryVM = DXCountryViewModel.Create();
                    countryVM.AttachModel((DXCountry)itemPair.Value);
                    Countries.Add(countryVM);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while loading data: " + ex.Message);
            }
        }

        public void LoadCountriesFromDb(CountryContext dbContext)
        {
            Countries.Clear();
            List<DXCountry> modelList = dbContext.Countries.Include(x => x.Flag).ToList();
            foreach (DXCountry countryModel in modelList)
            {
                DXCountryViewModel countryVM = DXCountryViewModel.Create();
                countryVM.AttachModel(countryModel);
                Countries.Add(countryVM);
            }
        }
    }
}