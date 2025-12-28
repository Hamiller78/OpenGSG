using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld
{
    internal class CwpEconomy(CwpCountry country) : IEconomy
    {
        public long TotalProduction { get; set; } = 0;
        public int Military { get; set; } = 30;
        public int Investment { get; set; } = 40;
        public int Wealth { get; set; } = 30;

        public void CalculateTotalProduction(WorldState worldstate)
        {
            var countryProvinces = country.Provinces.Cast<CwpProvince>();
            TotalProduction = countryProvinces.Sum(p => p.Production);
        }
    }
}
