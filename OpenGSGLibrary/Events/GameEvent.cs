using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// 0 means the event fires immediately when triggers are met.
        /// Higher values mean lower probability per day.
        /// </summary>
        public int MeanTimeToHappenDays { get; set; } = 0;

        /// <summary>
        /// The game tick (day) when triggers were first met.
        /// Used for MTTH calculation. -1 means triggers not yet met.
        /// </summary>
        private long _triggerMetTick = -1;

        /// <summary>
        /// Random number generator for MTTH rolls (static for consistent seeding).
        /// </summary>
        private static readonly Random _random = new Random();

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

                // Check if this is a date comparison trigger (key starts with "date")
                if (key.StartsWith("date"))
                {
                    foreach (var value in group)
                    {
                        var dateTrigger = ParseDateTrigger(key, value);
                        if (dateTrigger != null)
                        {
                            triggers.Add(dateTrigger);
                        }
                    }
                }
                else
                {
                    foreach (var value in group)
                    {
                        var trigger = ParseSingleTrigger(key, value);
                        if (trigger != null)
                        {
                            triggers.Add(trigger);
                        }
                    }
                }
            }

            return triggers;
        }

        /// <summary>
        /// Parses a date comparison trigger.
        /// The key will be "date>=" / "date>" / "date<" / "date<=" / "date"
        /// depending on the operator used in the event file.
        /// </summary>
        protected virtual IEventTrigger? ParseDateTrigger(string key, object value)
        {
            var dateStr = value?.ToString();
            if (string.IsNullOrEmpty(dateStr))
                return null;

            // Parse the date (format: YYYY.MM.DD)
            var date = ParseDate(dateStr);
            if (date == DateTime.MinValue)
                return null;

            // Extract operator from key suffix
            if (key == "date>=")
                return new DateGreaterEqualTrigger { Date = date };
            else if (key == "date>")
                return new DateGreaterThanTrigger { Date = date };
            else if (key == "date<")
                return new DateLessThanTrigger { Date = date };
            else if (key == "date<=")
                return new DateLessEqualTrigger { Date = date };
            else if (key == "date")
                return new DateEqualTrigger { Date = date };

            return null;
        }

        /// <summary>
        /// Parses a date string in format YYYY.MM.DD
        /// </summary>
        protected static DateTime ParseDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr))
                return DateTime.MinValue;

            // Format: 1950.01.09
            var parts = dateStr.Split('.');
            if (parts.Length != 3)
                return DateTime.MinValue;

            if (
                int.TryParse(parts[0], out var year)
                && int.TryParse(parts[1], out var month)
                && int.TryParse(parts[2], out var day)
            )
            {
                try
                {
                    return new DateTime(year, month, day);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

            return DateTime.MinValue;
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
        /// Evaluates whether this event should fire, considering both triggers and MTTH.
        /// Call this each tick when triggers are met to determine if the event fires.
        /// </summary>
        /// <param name="context">Game state context for evaluating triggers.</param>
        /// <param name="currentTick">Current game tick (day).</param>
        /// <returns>True if the event should fire this tick.</returns>
        public virtual bool ShouldFire(object context, long currentTick)
        {
            // Check if triggers are met
            if (!EvaluateTriggers(context))
            {
                // Triggers not met - reset MTTH tracking
                _triggerMetTick = -1;
                return false;
            }

            // Triggers are met

            // If no MTTH, fire immediately
            if (MeanTimeToHappenDays <= 0)
            {
                return true;
            }

            // Track when triggers were first met
            if (_triggerMetTick == -1)
            {
                _triggerMetTick = currentTick;
            }

            // Calculate probability to fire this tick
            // Formula: probability = 1 / MTTH per day
            // This gives approximately MTTH days average time to happen
            double fireChance = 1.0 / MeanTimeToHappenDays;

            // Roll the dice
            return _random.NextDouble() < fireChance;
        }

        /// <summary>
        /// Resets the MTTH tracking for this event.
        /// Called when the event fires or when it should be reset.
        /// </summary>
        public void ResetMTTH()
        {
            _triggerMetTick = -1;
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

            // Reset MTTH tracking after firing
            ResetMTTH();

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
