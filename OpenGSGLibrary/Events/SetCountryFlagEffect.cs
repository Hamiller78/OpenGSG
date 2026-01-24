namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Effect that sets a country flag.
    /// Flags persist for the entire game session and can be checked by triggers.
    /// </summary>
    public class SetCountryFlagEffect : IEventEffect
    {
        public string FlagName { get; set; } = string.Empty;

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
            {
                Console.WriteLine("ERROR: Context is not EventEvaluationContext");
                return;
            }

            Console.WriteLine(
                $"Setting flag '{FlagName}' for country '{evalContext.CurrentCountryTag}'"
            );

            var countries = evalContext.WorldState?.GetCountryTable();
            if (
                countries == null
                || !countries.TryGetValue(evalContext.CurrentCountryTag, out var country)
            )
            {
                Console.WriteLine(
                    $"ERROR: Could not find country '{evalContext.CurrentCountryTag}'"
                );
                return;
            }

            country.SetFlag(FlagName);
            Console.WriteLine(
                $"SUCCESS: Flag '{FlagName}' set for '{evalContext.CurrentCountryTag}'"
            );
        }
    }
}
