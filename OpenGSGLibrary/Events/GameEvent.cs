using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Sets the event properties from parsed data.
        /// Can be overridden by derived classes to handle game-specific properties.
        /// </summary>
        /// <param name="eventData">Parsed event data from file.</param>
        public virtual void SetData(ILookup<string, object> eventData)
        {
            Id = GetString(eventData, "id");
            Title = GetString(eventData, "title");
            Description = GetString(eventData, "desc");
            Picture = GetString(eventData, "picture");
            TriggerOnlyOnce = GetBool(eventData, "trigger_only_once", false);
            Hidden = GetBool(eventData, "hidden", false);

            // Parse mean_time_to_happen
            if (eventData.Contains("mean_time_to_happen"))
            {
                var mtth = eventData["mean_time_to_happen"].FirstOrDefault();
                if (mtth is ILookup<string, object> mtthData)
                {
                    MeanTimeToHappenDays = GetInt(mtthData, "days", 0);
                }
            }

            // Parse triggers
            if (eventData.Contains("trigger"))
            {
                var triggerData = eventData["trigger"].FirstOrDefault();
                if (triggerData is ILookup<string, object> triggers)
                {
                    Triggers.AddRange(ParseTriggers(triggers));
                }
            }

            // Parse options
            foreach (var optionData in eventData["option"])
            {
                if (optionData is ILookup<string, object> option)
                {
                    Options.Add(ParseOption(option));
                }
            }
        }

        /// <summary>
        /// Parses trigger blocks from event data.
        /// Can be overridden to add game-specific trigger types.
        /// </summary>
        protected virtual List<IEventTrigger> ParseTriggers(ILookup<string, object> triggerData)
        {
            var triggers = new List<IEventTrigger>();

            // Handle OR blocks
            if (triggerData.Contains("OR"))
            {
                var orData = triggerData["OR"].FirstOrDefault();
                if (orData is ILookup<string, object> orTriggers)
                {
                    var orTrigger = new OrTrigger();
                    orTrigger.Triggers.AddRange(ParseTriggers(orTriggers));
                    triggers.Add(orTrigger);
                }
            }

            // Handle AND blocks
            if (triggerData.Contains("AND"))
            {
                var andData = triggerData["AND"].FirstOrDefault();
                if (andData is ILookup<string, object> andTriggers)
                {
                    var andTrigger = new AndTrigger();
                    andTrigger.Triggers.AddRange(ParseTriggers(andTriggers));
                    triggers.Add(andTrigger);
                }
            }

            // Handle individual triggers
            foreach (var group in triggerData)
            {
                var key = group.Key;
                if (key == "OR" || key == "AND")
                    continue; // Already handled

                foreach (var value in group)
                {
                    var trigger = ParseSingleTrigger(key, value);
                    if (trigger != null)
                    {
                        triggers.Add(trigger);
                    }
                }
            }

            return triggers;
        }

        /// <summary>
        /// Parses a single trigger from event data.
        /// Can be overridden to add game-specific trigger types.
        /// </summary>
        protected virtual IEventTrigger? ParseSingleTrigger(string key, object value)
        {
            switch (key)
            {
                case "tag":
                    return new TagTrigger { Tag = value.ToString() ?? string.Empty };

                case "country_exists":
                    return new CountryExistsTrigger { Tag = value.ToString() ?? string.Empty };

                default:
                    // Check if this is a country scope trigger (e.g., "ROK = { has_war = no }")
                    if (value is ILookup<string, object> scopedTriggers)
                    {
                        return new CountryScopeTrigger
                        {
                            CountryTag = key,
                            ScopedTriggers = ParseTriggers(scopedTriggers),
                        };
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// Parses an event option from event data.
        /// Can be overridden to add game-specific effect types.
        /// </summary>
        protected virtual EventOption ParseOption(ILookup<string, object> optionData)
        {
            var option = new EventOption { Name = GetString(optionData, "name") };

            // Parse visible effects
            foreach (var group in optionData)
            {
                var key = group.Key;
                if (key == "name" || key == "hidden_effect")
                    continue;

                foreach (var value in group)
                {
                    var effect = ParseEffect(key, value);
                    if (effect != null)
                    {
                        option.Effects.Add(effect);
                    }
                }
            }

            // Parse hidden effects
            if (optionData.Contains("hidden_effect"))
            {
                var hiddenData = optionData["hidden_effect"].FirstOrDefault();
                if (hiddenData is ILookup<string, object> hiddenEffects)
                {
                    foreach (var group in hiddenEffects)
                    {
                        foreach (var value in group)
                        {
                            var effect = ParseEffect(group.Key, value);
                            if (effect != null)
                            {
                                option.HiddenEffects.Add(effect);
                            }
                        }
                    }
                }
            }

            return option;
        }

        /// <summary>
        /// Parses a single effect from event data.
        /// Must be overridden by derived classes to handle game-specific effects.
        /// </summary>
        protected virtual IEventEffect? ParseEffect(string key, object value)
        {
            switch (key)
            {
                case "news_event":
                    if (value is ILookup<string, object> newsEventData)
                    {
                        return new TriggerEventEffect
                        {
                            EventId = GetString(newsEventData, "id"),
                            IsNewsEvent = true,
                        };
                    }
                    break;

                case "country_event":
                    if (value is ILookup<string, object> countryEventData)
                    {
                        return new TriggerEventEffect
                        {
                            EventId = GetString(countryEventData, "id"),
                            IsCountryEvent = true,
                        };
                    }
                    break;
            }

            return null;
        }

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

        // Helper methods for parsing
        protected static string GetString(
            ILookup<string, object> data,
            string key,
            string defaultValue = ""
        )
        {
            if (!data.Contains(key))
                return defaultValue;

            var value = data[key].FirstOrDefault();
            return value?.ToString() ?? defaultValue;
        }

        protected static bool GetBool(
            ILookup<string, object> data,
            string key,
            bool defaultValue = false
        )
        {
            var str = GetString(data, key);
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            return str.ToLowerInvariant() == "yes" || str == "true";
        }

        protected static int GetInt(ILookup<string, object> data, string key, int defaultValue = 0)
        {
            var str = GetString(data, key);
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            return int.TryParse(str, out var result) ? result : defaultValue;
        }
    }
}
