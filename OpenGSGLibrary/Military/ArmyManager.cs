using OpenGSGLibrary.GameDataManager;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Manages armies loaded from data files and provides lookup by province.
    /// </summary>
    public class ArmyManager
    {
        /// <summary>
        /// Maps a province id to the list of armies currently located there.
        /// Populated by <see cref="UpdateProvinceToArmyTable"/> after loading units.
        /// </summary>
        private Dictionary<int, List<Army>>? provinceIdToArmiesTable_ = null;

        /// <summary>
        /// Table of nation military objects keyed by nation tag.
        /// Populated by <see cref="LoadFolder"/>.
        /// </summary>
        private Dictionary<string, NationMilitary>? nationMilitaryTable_ = null;

        /// <summary>
        /// Load nation military definitions from the given folder.
        /// This reads unit files using the <see cref="GameDataManager.GameObjectFactory"/> and
        /// rebuilds the province->armies index for fast lookup.
        /// </summary>
        /// <param name="unitsPath">File system path to the units folder.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown when the given folder does not exist.</exception>
        public void LoadFolder(string unitsPath)
        {
            if (!Directory.Exists(unitsPath))
                throw new DirectoryNotFoundException("Given game data directory not found: " + unitsPath);

            nationMilitaryTable_ = GameObjectFactory.FromFolder<string, NationMilitary, NationMilitary>(unitsPath, "tag");
            UpdateProvinceToArmyTable();
        }

        /// <summary>
        /// Returns the list of armies currently located in the specified province.
        /// Returns null when the army index has not been initialised.
        /// </summary>
        /// <param name="provinceId">Province identifier.</param>
        /// <returns>List of <see cref="Army"/> in the province or null.</returns>
        public List<Army>? GetArmiesInProvince(int provinceId)
        {
            if (provinceIdToArmiesTable_ == null)
                return null;

            provinceIdToArmiesTable_.TryGetValue(provinceId, out var resultList);
            return resultList;
        }

        /// <summary>
        /// Move an army from its current province to the target province and update the index.
        /// </summary>
        /// <param name="movingArmy">Army to move.</param>
        /// <param name="targetProvinceId">Target province id.</param>
        public void MoveArmy(Army movingArmy, int targetProvinceId)
        {
            var oldLocation = movingArmy.GetLocation();
            if (provinceIdToArmiesTable_ == null) return;

            var oldProvinceList = provinceIdToArmiesTable_[oldLocation];
            oldProvinceList.Remove(movingArmy);

            if (!provinceIdToArmiesTable_.TryGetValue(targetProvinceId, out var newProvinceList))
            {
                newProvinceList = new List<Army>();
                provinceIdToArmiesTable_.Add(targetProvinceId, newProvinceList);
            }

            movingArmy.SetLocation(targetProvinceId);
            newProvinceList.Add(movingArmy);
        }

        /// <summary>
        /// Rebuilds the lookup table from province id to armies based on the current
        /// <see cref="nationMilitaryTable_"/>. This is used after loading unit definitions
        /// to enable fast queries for armies per province.
        /// </summary>
        private void UpdateProvinceToArmyTable()
        {
            provinceIdToArmiesTable_ = new Dictionary<int, List<Army>>();
            if (nationMilitaryTable_ == null) return;

            foreach (var countryNationMil in nationMilitaryTable_)
            {
                foreach (var army in countryNationMil.Value.GetArmiesList())
                {
                    var location = army.GetLocation();
                    if (!provinceIdToArmiesTable_.TryGetValue(location, out var armyList))
                    {
                        armyList = new List<Army>();
                        provinceIdToArmiesTable_.Add(location, armyList);
                    }
                    armyList.Add(army);
                }
            }
        }
    }
}
