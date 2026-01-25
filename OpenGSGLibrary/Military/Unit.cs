namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Represents a unit type definition (infantry, armor, fighter, etc.).
    /// Loaded from common/units/*.txt files.
    /// </summary>
    public class Unit
    {
        /// <summary>
        /// Unit type identifier (matches filename, e.g., "infantry")
        /// </summary>
        public string TypeId { get; set; } = string.Empty;

        /// <summary>
        /// Branch type: "army" or "air"
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Combat strength per unit
        /// </summary>
        public int Strength { get; set; }

        /// <summary>
        /// Movement speed
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// Production cost to build one unit
        /// </summary>
        public int ProductionCost { get; set; }

        /// <summary>
        /// Maintenance cost per day/tick
        /// </summary>
        public int MaintenanceCost { get; set; }

        /// <summary>
        /// Loads unit definition from parsed data.
        /// </summary>
        public void SetData(string typeId, ILookup<string, object> parsedData)
        {
            TypeId = typeId;
            Type = GetString(parsedData, "type");
            Strength = GetInt(parsedData, "strength", 0);
            Speed = GetInt(parsedData, "speed", 0);
            ProductionCost = GetInt(parsedData, "production_cost", 0);
            MaintenanceCost = GetInt(parsedData, "maintenance_cost", 0);
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
