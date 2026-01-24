namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Trigger that checks if a country has a specific flag set.
    /// </summary>
    public class HasCountryFlagTrigger : IEventTrigger
    {
        public string FlagName { get; set; } = string.Empty;

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            if (string.IsNullOrEmpty(FlagName))
                return false;

            var countries = evalContext.WorldState?.GetCountryTable();
            if (
                countries == null
                || !countries.TryGetValue(evalContext.CurrentCountryTag, out var country)
            )
                return false;

            return country.HasFlag(FlagName);
        }
    }
}
