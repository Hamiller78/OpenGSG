using System;
using System.Linq;
using OpenGSGLibrary.GameLogic;
using OpenGSGLibrary.WorldData;

namespace WorldData
{
    /// <summary>
    /// Province class for cold war game. Adds specialised properties to base province class.
    /// </summary>
    public class CwpProvince : Province
    {
        public long population { get; set; } = 0;
        public long industrialization { get; set; } = 0;
        public long education { get; set; } = 0;
        public string terrain { get; set; } = string.Empty;

        public long production
        {
            get
            {
                CalculateProduction();
                return production_;
            }
        }
        private long production_ = 0;

        public CwpProvince()
            : base() { }

        /// <summary>
        /// Sets the province properties from the parsed data.
        /// </summary>
        /// <param name="fileName">Name of the source file of province object.</param>
        /// <param name="parsedData">Object with the parsed data from that file.</param>
        public override void SetData(string fileName, Lookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);

            population = ToLong(SingleOrDefault(parsedData, "population"));
            industrialization = ToLong(SingleOrDefault(parsedData, "industrialization"));
            education = ToLong(SingleOrDefault(parsedData, "education"));
            terrain = ToStringSafe(SingleOrDefault(parsedData, "terrain"));
        }

        /// <summary>
        /// Handler for game ticks.
        /// In this game, a tick represents a day.
        /// </summary>
        /// <param name="sender">sender TickHandler</param>
        /// <param name="e">TickEventArgs containing current game time</param>
        public override void OnTickDone(object sender, TickEventArgs e)
        {
            UpdateDaily();
        }

        private void UpdateDaily()
        {
            // Change in population number, small daily growth (preserve integer storage)
            population = Convert.ToInt64(Math.Round(population * 1.00003));
            // Change in industrialization
            // Change of education
        }

        public void CalculateProduction()
        {
            // Use floating arithmetic and convert to long like the original VB behaviour
            production_ = Convert.ToInt64(
                population * industrialization / 100.0 * education / 100.0
            );
        }

        // -- Helpers to safely extract single values from ILookup --
        private static object SingleOrDefault(ILookup<string, object> lookup, string key)
        {
            if (lookup == null)
                return null;
            return lookup.Contains(key) ? lookup[key].SingleOrDefault() : null;
        }

        private static long ToLong(object o)
        {
            if (o == null)
                return 0;
            try
            {
                return Convert.ToInt64(o);
            }
            catch
            {
                return 0;
            }
        }

        private static string ToStringSafe(object o) => o?.ToString() ?? string.Empty;
    }
}
