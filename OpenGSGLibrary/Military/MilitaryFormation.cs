namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Represents a military formation (army or air force) with unit counts.
    /// Loaded from history/units/*.txt files.
    /// </summary>
    public class MilitaryFormation
    {
        /// <summary>
        /// Branch type: "army" or "air_forces"
        /// </summary>
        public string Branch { get; set; } = string.Empty;

        /// <summary>
        /// Province ID where this formation is stationed
        /// </summary>
        public int Location { get; set; }

        /// <summary>
        /// Unit counts by type (e.g., "infantry" => 6, "armor" => 3)
        /// </summary>
        public Dictionary<string, int> Units { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Readiness state (active, ready, training, etc.)
        /// </summary>
        public string Readiness { get; set; } = "training";

        /// <summary>
        /// Loads formation from parsed data.
        /// </summary>
        public void SetData(string branch, ILookup<string, object> parsedData)
        {
            Branch = branch;
            Location = GetInt(parsedData, "location", 0);
            Readiness = GetString(parsedData, "readiness", "training");

            // Parse unit counts
            foreach (var group in parsedData)
            {
                var key = group.Key;

                // Skip non-unit properties
                if (key == "location" || key == "readiness")
                    continue;

                // Everything else is a unit type with count
                var count = GetInt(parsedData, key, 0);
                if (count > 0)
                {
                    Units[key] = count;
                }
            }
        }

        /// <summary>
        /// Calculates total strength of this formation.
        /// </summary>
        public int GetTotalStrength(Dictionary<string, Unit> unitDefinitions)
        {
            int total = 0;
            foreach (var (unitType, count) in Units)
            {
                if (unitDefinitions.TryGetValue(unitType, out var unitDef))
                {
                    total += unitDef.Strength * count;
                }
            }
            return total;
        }

        /// <summary>
        /// Calculates total maintenance cost.
        /// </summary>
        public int GetMaintenanceCost(Dictionary<string, Unit> unitDefinitions)
        {
            int total = 0;
            foreach (var (unitType, count) in Units)
            {
                if (unitDefinitions.TryGetValue(unitType, out var unitDef))
                {
                    total += unitDef.MaintenanceCost * count;
                }
            }
            return total;
        }

        private static string GetString(
            ILookup<string, object> data,
            string key,
            string defaultValue = ""
        )
        {
            if (!data.Contains(key))
                return defaultValue;
            return data[key].FirstOrDefault()?.ToString() ?? defaultValue;
        }

        private static int GetInt(ILookup<string, object> data, string key, int defaultValue = 0)
        {
            var str = GetString(data, key);
            return int.TryParse(str, out var result) ? result : defaultValue;
        }
    }
}
