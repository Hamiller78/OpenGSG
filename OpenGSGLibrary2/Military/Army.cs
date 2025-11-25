using System;
using System.Collections.Generic;

namespace Military
{
    /// <summary>
    /// Represents an army composed of divisions and belonging to a nation.
    /// The army can be located in a province and contains a list of divisions.
    /// </summary>
    public class Army : WorldData.GameObject
    {
        /// <summary>
        /// Publicly accessible list of divisions (nullable when not loaded via parser).
        /// </summary>
        public List<Division>? units { get; set; }

        private string owner_ = string.Empty;
        private string name_ = string.Empty;
        private int location_ = 0;
        private List<Division>? divisions_ = null;

        /// <summary>
        /// Populate army fields from parsed data. Expected keys: "name", "location" and nested "division" entries.
        /// </summary>
        /// <param name="fileName">Source file name (without extension) used as identifier.</param>
        /// <param name="parsedData">Lookup structure with parsed fields.</param>
        public override void SetData(string fileName, ILookup<string, object> parsedData)
        {
            name_ = parsedData.Contains("name") ? parsedData["name"].Single()?.ToString() ?? string.Empty : string.Empty;
            location_ = parsedData.Contains("location") ? Convert.ToInt32(parsedData["location"].Single()) : 0;
            divisions_ = WorldData.GameObjectFactory.ListFromLookup<Division>(parsedData, "division");
        }

        /// <summary>
        /// Assigns an owner tag to this army and propagates the owner to contained divisions.
        /// </summary>
        /// <param name="tag">Country tag string.</param>
        public void SetOwner(string tag)
        {
            owner_ = tag;
            if (divisions_ != null)
            {
                foreach (var division in divisions_) division.SetOwner(tag);
            }
        }

        /// <summary>
        /// Gets the province id this army is located in.
        /// </summary>
        public int GetLocation() => location_;

        /// <summary>
        /// Set the army's current province location id.
        /// </summary>
        /// <param name="provinceId">Target province id.</param>
        public void SetLocation(int provinceId) => location_ = provinceId;

        /// <summary>
        /// String representation of the army (its name).
        /// </summary>
        public override string ToString() => name_;
    }
}
