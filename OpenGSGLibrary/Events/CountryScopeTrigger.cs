namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Trigger that evaluates inner triggers in the context of a specific country.
    /// Used for scoped checks like: USA = { has_country_flag = my_flag }
    /// </summary>
    public class CountryScopeTrigger : IEventTrigger
    {
        public string CountryTag { get; set; } = string.Empty;
        public List<IEventTrigger> ScopedTriggers { get; set; } = new List<IEventTrigger>();

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            if (string.IsNullOrEmpty(CountryTag))
                return false;

            // Look up the scoped country
            var countries = evalContext.WorldState?.GetCountryTable();
            if (countries == null || !countries.TryGetValue(CountryTag, out var country))
                return false;

            // Create a new context with the scoped country as the current country
            var scopedContext = new EventEvaluationContext
            {
                WorldState = evalContext.WorldState,
                CurrentDate = evalContext.CurrentDate,
                CurrentCountryTag = CountryTag, // This is the key change
                TickHandler = evalContext.TickHandler,
                EventManager = evalContext.EventManager,
            };

            // Evaluate all inner triggers in the scoped context
            foreach (var trigger in ScopedTriggers)
            {
                if (!trigger.Evaluate(scopedContext))
                    return false;
            }

            return true;
        }
    }
}
