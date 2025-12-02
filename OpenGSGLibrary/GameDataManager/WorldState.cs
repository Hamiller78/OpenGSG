using OpenGSGLibrary.Military;
using System.Collections.Generic;

namespace OpenGSGLibrary.GameDataManager
{
    public class WorldState
    {
        private IDictionary<int, Province>? provinceTable_ = null;
        private IDictionary<string, Country>? countryTable_ = null;
        private ArmyManager? armyManager_ = new Military.ArmyManager();

        // Store orders as objects to avoid cross-assembly type resolving during incremental migration.
        private List<object>? runningOrders_ = new List<object>();

        public void SetProvinceTable(IDictionary<int, Province> provinceTable) => provinceTable_ = provinceTable;
        public IDictionary<int, Province>? GetProvinceTable() => provinceTable_;

        public void SetCountryTable(IDictionary<string, Country> countryTable) => countryTable_ = countryTable;
        public IDictionary<string, Country>? GetCountryTable() => countryTable_;

        public void SetArmyManager(ArmyManager armyManager) => armyManager_ = armyManager;
        public ArmyManager? GetArmyManager() => armyManager_;

        public List<object>? GetOrders() => runningOrders_;
    }
}
