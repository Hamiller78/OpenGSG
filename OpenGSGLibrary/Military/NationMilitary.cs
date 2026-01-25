using System.Collections.Generic;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Represents a country's military forces.
    /// </summary>
    public class NationMilitary
    {
        /// <summary>
        /// Country tag
        /// </summary>
        public string Tag { get; set; } = string.Empty;

        /// <summary>
        /// All armies belonging to this nation
        /// </summary>
        public List<MilitaryFormation> Armies { get; set; } = new List<MilitaryFormation>();

        /// <summary>
        /// All air forces belonging to this nation
        /// </summary>
        public List<MilitaryFormation> AirForces { get; set; } = new List<MilitaryFormation>();

        /// <summary>
        /// Loads military from parsed data.
        /// </summary>
        public void SetData(string tag, ILookup<string, object> parsedData)
        {
            Tag = tag;

            // Parse armies
            if (parsedData.Contains("army"))
            {
                foreach (var armyData in parsedData["army"])
                {
                    if (armyData is ILookup<string, object> armyLookup)
                    {
                        var army = new MilitaryFormation();
                        army.SetData("army", armyLookup);
                        Armies.Add(army);
                    }
                }
            }

            // Parse air forces
            if (parsedData.Contains("air_forces"))
            {
                foreach (var airData in parsedData["air_forces"])
                {
                    if (airData is ILookup<string, object> airLookup)
                    {
                        var airForce = new MilitaryFormation();
                        airForce.SetData("air_forces", airLookup);
                        AirForces.Add(airForce);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates total military strength.
        /// </summary>
        public int GetTotalStrength(Dictionary<string, Unit> unitDefinitions)
        {
            int total = 0;
            foreach (var army in Armies)
            {
                total += army.GetTotalStrength(unitDefinitions);
            }
            foreach (var airForce in AirForces)
            {
                total += airForce.GetTotalStrength(unitDefinitions);
            }
            return total;
        }

        /// <summary>
        /// Calculates total daily maintenance cost.
        /// </summary>
        public int GetTotalMaintenanceCost(Dictionary<string, Unit> unitDefinitions)
        {
            int total = 0;
            foreach (var army in Armies)
            {
                total += army.GetMaintenanceCost(unitDefinitions);
            }
            foreach (var airForce in AirForces)
            {
                total += airForce.GetMaintenanceCost(unitDefinitions);
            }
            return total;
        }
    }
}
