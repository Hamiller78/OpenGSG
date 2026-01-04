using System;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Triggers another event (typically a news event).
    /// This is a generic effect used in hidden_effect blocks.
    /// </summary>
    public class TriggerEventEffect : IEventEffect
    {
        public string EventId { get; set; } = string.Empty;
        public bool IsNewsEvent { get; set; } = false;
        public bool IsCountryEvent { get; set; } = false;

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            // TODO: Implement event manager to lookup and fire events by ID
            // Event manager should be passed in context
        }
    }
}
