using OpenGSGLibrary.Military;

namespace OpenGSGLibrary.GameDataManager
{
    public class WorldState
    {
        private IDictionary<int, Province> _provinceTable = default!;
        private IDictionary<string, Country> _countryTable = default!;
        private ArmyManager _armyManager = new();

        // Store orders as objects to avoid cross-assembly type resolving during incremental migration.
        private readonly List<object> _runningOrders = [];

        public void SetProvinceTable(IDictionary<int, Province> provinceTable) =>
            _provinceTable = provinceTable;

        public IDictionary<int, Province>? GetProvinceTable() => _provinceTable;

        public void SetCountryTable(IDictionary<string, Country> countryTable) =>
            _countryTable = countryTable;

        public IDictionary<string, Country>? GetCountryTable() => _countryTable;

        public void SetArmyManager(ArmyManager armyManager) => _armyManager = armyManager;

        public ArmyManager? GetArmyManager() => _armyManager;

        public List<object>? GetOrders() => _runningOrders;
    }
}
