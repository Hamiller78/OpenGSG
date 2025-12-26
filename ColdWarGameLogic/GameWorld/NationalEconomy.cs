using System;
using System.Collections.Generic;
using System.Text;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld
{
    internal class NationalEconomy
    {
        private readonly CwpCountry _country;
        public long TotalProduction { get; set; } = 0;
        public int Military { get; set; }
        public int Investment { get; set; }
        public int Wealth { get; set; }

        public NationalEconomy(CwpCountry country)
        {
            _country = country;
        }

        public void CalculateTotalProduction(WorldState worldstate)
        {
            long totalProduction = 0;
            var worldProvinces = worldstate.GetProvinceTable().Values.Cast<CwpProvince>();
            var countryProvinces = worldProvinces.Where(p => p.Owner == _country.Tag);

            foreach (var province in countryProvinces)
            {
                if (province is CwpProvince cwpProvince)
                {
                    totalProduction += cwpProvince.Production;
                }
            }
            TotalProduction = totalProduction;
        }
    }
}
