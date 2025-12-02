using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.GameWorld
{
    /// <summary>
    /// Country class for cold war game. Adds specialised properties to base country class.
    /// </summary>
    public class CwpCountry : Country
    {
        public string longName { get; set; } = string.Empty;
        public string government { get; set; } = string.Empty;
        public string allegiance { get; set; } = string.Empty;
        public string leader { get; set; } = string.Empty;

        public CwpCountry()
            : base() { }

        /// <summary>
        /// Sets the country properties from the parsed data.
        /// </summary>
        /// <param name="fileName">Name of the source file of country object.</param>
        /// <param name="parsedData">Object with the parsed data from that file.</param>
        public override void SetData(
            string fileName,
            System.Linq.ILookup<string, object> parsedData
        )
        {
            base.SetData(fileName, parsedData);

            longName = parsedData["long_name"].Single()?.ToString() ?? string.Empty;
            government = parsedData["government"].Single()?.ToString() ?? string.Empty;
            if (parsedData.Contains("allegiance"))
            {
                allegiance = parsedData["allegiance"].Single()?.ToString() ?? string.Empty;
            }
            else
            {
                allegiance = string.Empty;
            }
            leader = parsedData["leader"].Single()?.ToString() ?? string.Empty;
        }
    }
}
