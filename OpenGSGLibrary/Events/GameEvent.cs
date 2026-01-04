using System;
using System.Collections.Generic;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Base class for all game events. Represents events that can be triggered during gameplay
    /// based on specific conditions (triggers) and offer choices (options) to the player.
    /// </summary>
    public abstract class GameEvent
    {
        /// <summary>
        /// Unique identifier for this event.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Localization key for the event title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Localization key for the event description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Picture/image identifier to display with this event.
        /// </summary>
        public string Picture { get; set; } = string.Empty;

        /// <summary>
        /// If true, this event can only trigger once per game.
        /// </summary>
        public bool TriggerOnlyOnce { get; set; } = false;

        /// <summary>
        /// If true, this event is hidden from the player and executes automatically.
        /// </summary>
        public bool Hidden { get; set; } = false;

        /// <summary>
        /// Has this event already been triggered (for trigger_only_once events)?
        /// </summary>
        public bool HasTriggered { get; set; } = false;

        /// <summary>
        /// List of trigger conditions for this event.
        /// All triggers must evaluate to true for the event to fire.
        /// </summary>
        public List<IEventTrigger> Triggers { get; set; } = new List<IEventTrigger>();

        /// <summary>
        /// List of options/choices available when this event fires.
        /// </summary>
        public List<EventOption> Options { get; set; } = new List<EventOption>();

        /// <summary>
        /// Mean time to happen in days (for random event triggering).
        /// </summary>
        public int MeanTimeToHappenDays { get; set; } = 0;

        /// <summary>
        /// Evaluates whether this event's trigger conditions are currently met.
        /// </summary>
        /// <param name="context">Game state context for evaluating triggers.</param>
        /// <returns>True if all trigger conditions are met.</returns>
        public virtual bool EvaluateTriggers(object context)
        {
            if (TriggerOnlyOnce && HasTriggered)
                return false;

            foreach (var trigger in Triggers)
            {
                if (!trigger.Evaluate(context))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Executes the event, typically presenting it to the player or auto-executing if hidden.
        /// </summary>
        /// <param name="context">Game state context.</param>
        public virtual void Fire(object context)
        {
            if (TriggerOnlyOnce)
            {
                HasTriggered = true;
            }

            if (Hidden && Options.Count > 0)
            {
                // Auto-execute first option for hidden events
                Options[0].Execute(context);
            }
            // Otherwise, UI should present the event to the player
        }
    }
}
