using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld
{
    /// <summary>
    /// Country class for cold war game. Adds specialised properties to base country class.
    /// </summary>
    public class CwpCountry : Country
    {
        public string FullName { get; set; } = string.Empty;
        public string Government { get; set; } = string.Empty;
        public string Allegiance { get; set; } = string.Empty;
        public string Leader { get; set; } = string.Empty;

        public CwpCountry()
            : base() { }

        /// <summary>
        /// Sets the country properties from the parsed data.
        /// </summary>
        /// <param name="fileName">Name of the source file of country object.</param>
        /// <param name="parsedData">Object with the parsed data from that file.</param>
        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);

            // Only load historical data if present (supports split loading from common + history)
            if (parsedData.Contains("long_name"))
            {
                FullName = parsedData["long_name"].Single()?.ToString() ?? string.Empty;
            }

            if (parsedData.Contains("government"))
            {
                Government = parsedData["government"].Single()?.ToString() ?? string.Empty;
            }

            if (parsedData.Contains("allegiance"))
            {
                Allegiance = parsedData["allegiance"].Single()?.ToString() ?? string.Empty;
            }

            if (parsedData.Contains("leader"))
            {
                Leader = parsedData["leader"].Single()?.ToString() ?? string.Empty;
            }

            // Initialize economy if not already present
            Economy ??= new CwpEconomy(this);
        }

        public override void OnTickDone(object sender, EventArgs e)
        {
            Economy.GrowProvinceIndustrialization();
            base.OnTickDone(sender, e);
        }
    }
}
