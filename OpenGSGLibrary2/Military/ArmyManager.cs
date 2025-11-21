using System;
using System.Collections.Generic;
using System.IO;

namespace Military
{
    public class ArmyManager
    {
        private Dictionary<int, List<Army>>? provinceIdToArmiesTable_ = null;
        private Dictionary<string, NationMilitary>? nationMilitaryTable_ = null;

        public void LoadFolder(string unitsPath)
        {
            if (!Directory.Exists(unitsPath))
                throw new DirectoryNotFoundException("Given game data directory not found: " + unitsPath);

            nationMilitaryTable_ = WorldData.GameObjectFactory.FromFolder<string, NationMilitary, NationMilitary>(unitsPath, "tag");
            UpdateProvinceToArmyTable();
        }

        public List<Army>? GetArmiesInProvince(int provinceId)
        {
            if (provinceIdToArmiesTable_ == null)
                return null;

            provinceIdToArmiesTable_.TryGetValue(provinceId, out var resultList);
            return resultList;
        }

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
