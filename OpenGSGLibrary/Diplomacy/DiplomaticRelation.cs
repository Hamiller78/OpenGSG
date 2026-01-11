using System;

namespace OpenGSGLibrary.Diplomacy
{
    /// <summary>
    /// Base class for diplomatic relations between countries.
    /// Represents a relationship from one country to another (e.g., war, alliance, guarantee).
    /// </summary>
    public abstract class DiplomaticRelation
    {
        /// <summary>
        /// Tag of the country that has this relation (the "from" country).
        /// </summary>
        public string FromCountryTag { get; set; } = string.Empty;

        /// <summary>
        /// Tag of the target country (the "to" country).
        /// </summary>
        public string ToCountryTag { get; set; } = string.Empty;

        /// <summary>
        /// Date when this relation started (optional).
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Date when this relation ends (optional, null for indefinite).
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Type identifier for this relation (e.g., "war", "alliance", "guarantee").
        /// Used for quick lookups and serialization.
        /// </summary>
        public abstract string RelationType { get; }

        /// <summary>
        /// Checks if this relation is currently active based on the given date.
        /// </summary>
        public virtual bool IsActive(DateTime currentDate)
        {
            if (StartDate.HasValue && currentDate < StartDate.Value)
                return false;

            if (EndDate.HasValue && currentDate >= EndDate.Value)
                return false;

            return true;
        }
    }
}
