using System;
using ColdWarGameLogic.GameWorld;
using OpenGSGLibrary.Events;
using OpenGSGLibrary.GameDataManager;

namespace ColdWarGameLogic.Events
{
    /// <summary>
    /// Effect that removes a guarantee from one country to another.
    /// Cold War specific - requires guarantee system in CwpCountry.
    /// </summary>
    public class RemoveGuaranteeEffect : IEventEffect
    {
        public string TargetCountryTag { get; set; } = string.Empty;

        public void Execute(WorldState state)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            var countries = evalContext.WorldState?.GetCountryTable();
            if (countries == null || string.IsNullOrEmpty(evalContext.CurrentCountryTag))
                return;

            if (!countries.TryGetValue(evalContext.CurrentCountryTag, out var country))
                return;

            if (country is not CwpCountry cwpCountry)
                return;

            // TODO: Implement guarantee system in CwpCountry class
            // cwpCountry.RemoveGuarantee(TargetCountryTag);
        }
    }

    /// <summary>
    /// Triggers another event (typically a news event).
    /// </summary>
    public class TriggerEventEffect : IEventEffect
    {
        public string EventId { get; set; } = string.Empty;
        public bool Hidden { get; set; } = false;

        public void Execute(object context)
        {
            if (context is not WorldState state)
                return;

            // TODO: Implement event manager to lookup and fire events by ID
            // For now, this is a placeholder
        }
    }
}
