namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Effect that clears a country flag.
    /// </summary>
    public class ClearCountryFlagEffect : IEventEffect
    {
        public string FlagName { get; set; } = string.Empty;

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            if (string.IsNullOrEmpty(FlagName))
                return;

            var countries = evalContext.WorldState?.GetCountryTable();
            if (
                countries == null
                || !countries.TryGetValue(evalContext.CurrentCountryTag, out var country)
            )
                return;

            country.ClearFlag(FlagName);
        }
    }
}
