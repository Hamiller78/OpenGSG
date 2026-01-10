namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Scoped trigger - evaluates triggers in the context of a specific country.
    /// Example: "ROK = { has_war = no }" evaluates has_war trigger for ROK.
    /// </summary>
    public class CountryScopeTrigger : IEventTrigger
    {
        public string CountryTag { get; set; } = string.Empty;
        public List<IEventTrigger> ScopedTriggers { get; set; } = new List<IEventTrigger>();

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            // Create a new context with the scoped country as current
            var scopedContext = new EventEvaluationContext
            {
                WorldState = evalContext.WorldState,
                CurrentDate = evalContext.CurrentDate,
                CurrentCountryTag = CountryTag,
                TickHandler = evalContext.TickHandler,
            };

            foreach (var trigger in ScopedTriggers)
            {
                if (!trigger.Evaluate(scopedContext))
                    return false;
            }

            return true;
        }
    }
}
