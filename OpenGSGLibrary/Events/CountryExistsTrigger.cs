namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Checks if a country with the given tag exists in the game.
    /// </summary>
    public class CountryExistsTrigger : IEventTrigger
    {
        public string Tag { get; set; } = string.Empty;

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            var countries = evalContext.WorldState?.GetCountryTable();
            return countries != null && countries.ContainsKey(Tag);
        }
    }
}
