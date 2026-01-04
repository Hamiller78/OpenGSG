using System;
using System.Collections.Generic;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Base interface for event trigger conditions.
    /// Triggers determine when an event can fire.
    /// </summary>
    public interface IEventTrigger
    {
        /// <summary>
        /// Evaluates whether this trigger condition is currently met.
        /// </summary>
        /// <param name="context">Game state context for evaluation.</param>
        /// <returns>True if the trigger condition is satisfied.</returns>
        bool Evaluate(object context);
    }

    /// <summary>
    /// Combines multiple triggers with AND logic.
    /// </summary>
    public class AndTrigger : IEventTrigger
    {
        public List<IEventTrigger> Triggers { get; set; } = new List<IEventTrigger>();

        public bool Evaluate(object context)
        {
            foreach (var trigger in Triggers)
            {
                if (!trigger.Evaluate(context))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Combines multiple triggers with OR logic.
    /// </summary>
    public class OrTrigger : IEventTrigger
    {
        public List<IEventTrigger> Triggers { get; set; } = new List<IEventTrigger>();

        public bool Evaluate(object context)
        {
            foreach (var trigger in Triggers)
            {
                if (trigger.Evaluate(context))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Inverts the result of another trigger (NOT logic).
    /// </summary>
    public class NotTrigger : IEventTrigger
    {
        public IEventTrigger? Trigger { get; set; }

        public bool Evaluate(object context)
        {
            return Trigger != null && !Trigger.Evaluate(context);
        }
    }
}
