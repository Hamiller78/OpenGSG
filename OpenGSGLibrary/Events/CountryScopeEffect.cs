namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Effect that executes inner effects in the context of a specific country.
    /// Used for scoped effects like: USSR = { set_country_flag = my_flag }
    /// </summary>
    public class CountryScopeEffect : IEventEffect
    {
        public string CountryTag { get; set; } = string.Empty;
        public List<IEventEffect> InnerEffects { get; set; } = new List<IEventEffect>();

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            // Create a new context with the scoped country
            var scopedContext = new EventEvaluationContext
            {
                WorldState = evalContext.WorldState,
                CurrentDate = evalContext.CurrentDate,
                CurrentCountryTag = CountryTag, // Execute effects for this country
                TickHandler = evalContext.TickHandler,
                EventManager = evalContext.EventManager,
            };

            // Execute all inner effects in the scoped context
            foreach (var effect in InnerEffects)
            {
                effect.Execute(scopedContext);
            }
        }
    }
}
