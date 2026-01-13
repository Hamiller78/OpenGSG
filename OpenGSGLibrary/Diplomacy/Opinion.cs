using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGSGLibrary.Diplomacy
{
    /// <summary>
    /// Represents one country's opinion of another country.
    /// Opinion values range from -100 (hostile) to +100 (friendly).
    /// </summary>
    public class Opinion
    {
        /// <summary>
        /// Tag of the country that holds this opinion.
        /// </summary>
        public string FromCountryTag { get; set; } = string.Empty;

        /// <summary>
        /// Tag of the country this opinion is about.
        /// </summary>
        public string TowardCountryTag { get; set; } = string.Empty;

        /// <summary>
        /// Base opinion value set in history files or by events.
        /// Range: -100 to +100
        /// </summary>
        public int BaseValue { get; set; } = 0;

        /// <summary>
        /// List of modifiers that affect the opinion (for future use).
        /// Each modifier has a reason and a value.
        /// </summary>
        public List<OpinionModifier> Modifiers { get; } = new List<OpinionModifier>();

        /// <summary>
        /// Gets the total opinion value (base + all active modifiers).
        /// </summary>
        public int GetTotalOpinion(DateTime currentDate)
        {
            var total = BaseValue;

            foreach (var modifier in Modifiers)
            {
                if (modifier.IsActive(currentDate))
                {
                    total += modifier.Value;
                }
            }

            // Clamp to -100 to +100 range
            return Math.Clamp(total, -100, 100);
        }

        /// <summary>
        /// Adds a modifier to this opinion.
        /// </summary>
        public void AddModifier(OpinionModifier modifier)
        {
            Modifiers.Add(modifier);
        }

        /// <summary>
        /// Removes all modifiers with a specific reason.
        /// </summary>
        public void RemoveModifiersByReason(string reason)
        {
            Modifiers.RemoveAll(m => m.Reason == reason);
        }
    }

    /// <summary>
    /// Represents a modifier to opinion (e.g., from events, diplomatic actions).
    /// </summary>
    public class OpinionModifier
    {
        /// <summary>
        /// Reason for this modifier (e.g., "Historical rivalry", "Trade agreement").
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Opinion value change (can be positive or negative).
        /// </summary>
        public int Value { get; set; } = 0;

        /// <summary>
        /// Optional: When this modifier starts being active.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Optional: When this modifier expires (null = permanent).
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Checks if this modifier is currently active.
        /// </summary>
        public bool IsActive(DateTime currentDate)
        {
            if (StartDate.HasValue && currentDate < StartDate.Value)
                return false;

            if (ExpiryDate.HasValue && currentDate >= ExpiryDate.Value)
                return false;

            return true;
        }
    }
}
