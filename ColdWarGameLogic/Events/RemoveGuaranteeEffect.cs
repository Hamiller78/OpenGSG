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

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            var countries = evalContext.WorldState?.GetCountryTable();
            if (countries == null || string.IsNullOrEmpty(evalContext.CurrentCountryTag))
                return;

            if (!countries.TryGetValue(evalContext.CurrentCountryTag, out var country))
                return;

            if (country is CwpCountry cwpCountry)
            {
                cwpCountry.RemoveGuarantee(TargetCountryTag);
            }
        }
    }
}
