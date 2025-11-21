using System.Collections.Generic;

namespace WorldData
{
    public class WorldState
    {
        private IDictionary<int, Province>? provinceTable_ = null;
        private IDictionary<string, Country>? countryTable_ = null;
        private Military.ArmyManager? armyManager_ = new Military.ArmyManager();

        private List<Orders.Order>? runningOrders_ = new List<Orders.Order>();

        public void SetProvinceTable(IDictionary<int, Province> provinceTable) => provinceTable_ = provinceTable;
        public IDictionary<int, Province>? GetProvinceTable() => provinceTable_;

        public void SetCountryTable(IDictionary<string, Country> countryTable) => countryTable_ = countryTable;
        public IDictionary<string, Country>? GetCountryTable() => countryTable_;

        public void SetArmyManager(Military.ArmyManager armyManager) => armyManager_ = armyManager;
        public Military.ArmyManager? GetArmyManager() => armyManager_;

        public List<Orders.Order>? GetOrders() => runningOrders_;
    }
}
