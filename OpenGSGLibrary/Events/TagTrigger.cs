using System;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Checks if the current country matches a specific tag.
    /// Used in event scoping (e.g., "tag = USA" means event is for USA).
    /// </summary>
    public class TagTrigger : IEventTrigger
    {
        public string Tag { get; set; } = string.Empty;

        public bool Evaluate(object context)
        {
            // Context should include information about which country is being evaluated
            // Implementation depends on EventEvaluationContext structure
            if (context is EventEvaluationContext evalContext)
            {
                return evalContext.CurrentCountryTag == Tag;
            }
            return false;
        }
    }
}
