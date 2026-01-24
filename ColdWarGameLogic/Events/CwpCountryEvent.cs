using System;
using System.Linq;
using OpenGSGLibrary.Events;

namespace ColdWarGameLogic.Events
{
    /// <summary>
    /// Cold War specific country event.
    /// Extends base CountryEvent with game-specific trigger and effect parsing.
    /// </summary>
    public class CwpCountryEvent : CountryEvent
    {
        /// <summary>
        /// Parses game-specific triggers (like has_war).
        /// </summary>
        protected override IEventTrigger? ParseSingleTrigger(string key, object value)
        {
            // Try base triggers first
            var baseTrigger = base.ParseSingleTrigger(key, value);
            if (baseTrigger != null)
                return baseTrigger;

            // Handle Cold War specific triggers
            // TODO: Add game-specific triggers like has_war when needed
            return null;
        }

        /// <summary>
        /// Parses game-specific effects (like remove_guarantee).
        /// </summary>
        protected override IEventEffect? ParseSingleEffect(string key, object value)
        {
            // Try base effects first
            var baseEffect = base.ParseSingleEffect(key, value);
            if (baseEffect != null)
                return baseEffect;

            // Handle Cold War specific effects
            switch (key)
            {
                case "remove_guarantee":
                    return new RemoveGuaranteeEffect
                    {
                        TargetCountryTag = value.ToString() ?? string.Empty,
                    };
            }

            return null;
        }
    }

    /// <summary>
    /// Cold War specific news event.
    /// </summary>
    public class CwpNewsEvent : NewsEvent
    {
        /// <summary>
        /// Parses game-specific triggers.
        /// </summary>
        protected override IEventTrigger? ParseSingleTrigger(string key, object value)
        {
            // Try base triggers first
            var baseTrigger = base.ParseSingleTrigger(key, value);
            if (baseTrigger != null)
                return baseTrigger;

            // Handle Cold War specific triggers
            // TODO: Add game-specific triggers when needed
            return null;
        }

        /// <summary>
        /// Parses game-specific effects.
        /// </summary>
        protected override IEventEffect? ParseSingleEffect(string key, object value)
        {
            // Try base effects first
            var baseEffect = base.ParseSingleEffect(key, value);
            if (baseEffect != null)
                return baseEffect;

            // Handle Cold War specific effects (none yet)
            return null;
        }
    }
}
