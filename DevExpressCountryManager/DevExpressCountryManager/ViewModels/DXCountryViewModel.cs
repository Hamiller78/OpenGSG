using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpressCountryManager.Database;
using DevExpressCountryManager.Models.Common;
using DevExpressCountryManager.Models.WorldData;

namespace DevExpressCountryManager.ViewModels
{
    // Test of DevExpress POCO ViewModels
    public class DXCountryViewModel
    {
        protected DXCountryViewModel()
        {
            _countryModel = new DXCountry();
        }

        public static DXCountryViewModel Create()
        {
            return ViewModelSource.Create(() => new DXCountryViewModel());
        }

        public virtual string Tag { get; set; }
        public virtual string Name { get; set; }
        public virtual string LongName { get; set; }
        public virtual string Government { get; set; }
        public virtual string Leader { get; set; }
        public virtual string Allegiance { get; set; }
        public virtual BlobbableImage Flag { get; set; }

        private DXCountry _countryModel;

        public void LoadFromDbByTag(string tag)
        {
            CountryContext dbContext = new CountryContext();

            DXCountry dbCountry = dbContext.Countries.Include(x => x.Tag == tag).First();
            if (dbCountry != null)
            {
                AttachModel(dbCountry);
            }
        }

        public void AttachModel(DXCountry country)
        {
            _countryModel = country;
            Tag = _countryModel.Tag;
            Name = _countryModel.Name;
            LongName = _countryModel.LongName;
            Government = _countryModel.Government;
            Leader = _countryModel.Leader;
            Allegiance = _countryModel.Allegiance;
            Flag = _countryModel.Flag;
        }

        public void SaveToDb()
        {
            UpdateModel();

            CountryContext dbContext = new CountryContext();

            string countryTag = _countryModel.Tag;
            DXCountry dbCountry = dbContext.Find<DXCountry>(countryTag);
            if (dbCountry == null)
            {
                dbContext.Add(_countryModel);
            }
            else
            {
                dbCountry = _countryModel;
            }
            int stateChanges = dbContext.SaveChanges();
            Console.WriteLine($"Country saved. {stateChanges} state changes written!");
        }

        private void UpdateModel()
        {
            if (_countryModel == null)
            {
                _countryModel = new DXCountry();
            }

            _countryModel.Tag = Tag;
            _countryModel.Name = Name;
            _countryModel.LongName = LongName;
            _countryModel.Government = Government;
            _countryModel.Leader = Leader;
            _countryModel.Allegiance = Allegiance;
            _countryModel.Flag = Flag;
        }

    }
}
