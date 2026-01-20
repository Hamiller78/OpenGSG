using System;
using System.Collections.Generic;
using System.Linq;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Responsible for evaluating and firing events based on triggers.
    /// </summary>
    public class EventEvaluator
    {
        private readonly EventManager _eventManager;

        public EventEvaluator(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        /// <summary>
        /// Evaluates all events and fires those whose triggers are met.
        /// Shows UI for active player country, auto-executes for AI countries.
        /// </summary>
        public void EvaluateAllEvents(
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag
        )
        {
            var countries =
                worldState.GetCountryTable()
                ?? throw new WorldStateException("Country table is null");

            // Evaluate country events for ALL countries
            foreach (var countryTag in countries.Values.Select(x => x.Tag))
            {
                var context = new EventEvaluationContext
                {
                    WorldState = worldState,
                    CurrentDate = currentDate,
                    CurrentCountryTag = countryTag,
                    TickHandler = null, // Will be set by caller if needed
                    EventManager = _eventManager,
                };

                EvaluateCountryEventsForCountry(
                    currentTick,
                    countryTag,
                    activePlayerCountryTag,
                    context
                );
            }

            // Evaluate news events
            EvaluateNewsEvents(
                worldState,
                currentDate,
                currentTick,
                activePlayerCountryTag,
                countries
            );
        }

        /// <summary>
        /// Fires an event immediately for a specific country, bypassing trigger evaluation.
        /// Used for debugging and testing.
        /// </summary>
        public bool FireEventDebug(
            string eventId,
            string countryTag,
            WorldState worldState,
            DateTime currentDate
        )
        {
            var countries = worldState.GetCountryTable();
            if (countries == null || !countries.TryGetValue(countryTag, out var country))
                return false;

            var countryEvents = _eventManager.GetCountryEvents();
            var newsEvents = _eventManager.GetNewsEvents();

            var evt =
                countryEvents.FirstOrDefault(e => e.Id == eventId)
                ?? (GameEvent?)newsEvents.FirstOrDefault(e => e.Id == eventId);

            if (evt == null)
                return false;

            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentDate = currentDate,
                CurrentCountryTag = countryTag,
                TickHandler = null, // Will be set by caller if needed
                EventManager = _eventManager,
            };

            FireEvent(evt, context);
            return true;
        }

        private void EvaluateCountryEventsForCountry(
            long currentTick,
            string countryTag,
            string? activePlayerCountryTag,
            EventEvaluationContext context
        )
        {
            var countryEvents = _eventManager.GetCountryEvents();
            foreach (var evt in countryEvents)
            {
                if (evt.IsTriggeredOnly)
                    continue;

                if (evt.ShouldFire(context, currentTick))
                {
                    if (evt.TriggerOnlyOnce)
                    {
                        evt.HasTriggered = true;
                    }

                    bool isPlayerCountry = countryTag == activePlayerCountryTag;

                    if (evt.Hidden)
                    {
                        if (evt.Options.Count > 0)
                        {
                            evt.Options[0].Execute(context);
                        }
                        evt.ResetMTTH();
                    }
                    else if (!isPlayerCountry)
                    {
                        // AI decision-making: for now, just pick first option
                        if (evt.Options.Count > 0)
                        {
                            evt.Options[0].Execute(context);
                        }
                        evt.ResetMTTH();
                    }
                    else
                    {
                        // Player country: show UI popup
                        TickHandler.TriggerEvent(evt, context);
                    }
                }
            }
        }

        private void EvaluateNewsEvents(
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag,
            IDictionary<string, Country> countries
        )
        {
            var newsEvents = _eventManager.GetNewsEvents();
            foreach (var evt in newsEvents)
            {
                if (evt.IsTriggeredOnly)
                    continue;

                bool hasCountrySpecificTriggers = evt.Triggers.Any(t =>
                    t is TagTrigger || t is CountryScopeTrigger
                );

                if (hasCountrySpecificTriggers)
                {
                    EvaluateCountrySpecificNewsEvent(
                        evt,
                        worldState,
                        currentDate,
                        currentTick,
                        activePlayerCountryTag,
                        countries
                    );
                }
                else
                {
                    EvaluateGlobalNewsEvent(
                        evt,
                        worldState,
                        currentDate,
                        currentTick,
                        activePlayerCountryTag
                    );
                }
            }
        }

        private void EvaluateCountrySpecificNewsEvent(
            GameEvent evt,
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag,
            IDictionary<string, Country> countries
        )
        {
            foreach (var countryTag in countries.Values.Select(x => x.Tag))
            {
                var context = new EventEvaluationContext
                {
                    WorldState = worldState,
                    CurrentDate = currentDate,
                    CurrentCountryTag = countryTag,
                    TickHandler = null,
                    EventManager = _eventManager,
                };

                if (evt.ShouldFire(context, currentTick))
                {
                    if (evt.TriggerOnlyOnce)
                    {
                        evt.HasTriggered = true;
                    }

                    bool isPlayerCountry = countryTag == activePlayerCountryTag;

                    if (evt.Hidden || !isPlayerCountry)
                    {
                        if (evt.Options.Count > 0)
                        {
                            evt.Options[0].Execute(context);
                        }
                        evt.ResetMTTH();
                    }
                    else
                    {
                        TickHandler.TriggerEvent(evt, context);
                    }
                }
            }
        }

        private void EvaluateGlobalNewsEvent(
            GameEvent evt,
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag
        )
        {
            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentDate = currentDate,
                CurrentCountryTag = activePlayerCountryTag,
                TickHandler = null,
                EventManager = _eventManager,
            };

            if (evt.ShouldFire(context, currentTick))
            {
                if (evt.TriggerOnlyOnce)
                {
                    evt.HasTriggered = true;
                }

                if (evt.Hidden)
                {
                    if (evt.Options.Count > 0)
                    {
                        evt.Options[0].Execute(context);
                    }
                    evt.ResetMTTH();
                }
                else if (!string.IsNullOrEmpty(activePlayerCountryTag))
                {
                    TickHandler.TriggerEvent(evt, context);
                }
            }
        }

        private void FireEvent(GameEvent evt, EventEvaluationContext context)
        {
            if (evt.Hidden)
            {
                if (evt.Options.Count > 0)
                {
                    evt.Options[0].Execute(context);
                }
            }
            else
            {
                TickHandler.TriggerEvent(evt, context);
            }
        }
    }
}
