using OpenGSGLibrary.GameDataManager;
using System.Collections.Generic;

namespace OpenGSGLibrary.Military
{
    /// <summary>
    /// Represents the military forces of a single nation. Contains a list of armies.
    /// </summary>
    public class NationMilitary : GameObject
    {
        private string owner_ = string.Empty;
        private List<Army> armies_ = new List<Army>();

        /// <summary>
        /// Populate nation military from parsed data. Expects a top-level "tag" and nested "army" entries.
        /// </summary>
        /// <param name="fileName">Source file name (unused).</param>
        /// <param name="parsedData">Parser output.</param>
        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            base.SetData(fileName, parsedData);
            owner_ = parsedData.Contains("tag") ? parsedData["tag"].Single()?.ToString() ?? string.Empty : string.Empty;
            armies_ = GameObjectFactory.ListFromLookup<Army>(parsedData, "army");
            foreach (var army in armies_) army.SetOwner(owner_);
        }

        /// <summary>
        /// Returns the list of armies belonging to this nation.
        /// </summary>
        public List<Army> GetArmiesList() => armies_;
    }
}
