using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Responsible for evaluating and firing events based on triggers.
    /// </summary>
    public class EventEvaluator(EventManager eventManager)
    {
        /// <summary>
        /// Evaluates all events and fires those whose triggers are met.
        /// </summary>
        public void EvaluateAllEvents(
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag,
            TickHandler tickHandler
        )
        {
            var countries =
                worldState.GetCountryTable()
                ?? throw new WorldStateException("Country table is null");

            // Evaluate country events
            EvaluateEventList(
                eventManager.GetCountryEvents(),
                worldState,
                currentDate,
                currentTick,
                activePlayerCountryTag,
                tickHandler,
                countries
            );

            // Evaluate news events (same logic)
            EvaluateEventList(
                eventManager.GetNewsEvents(),
                worldState,
                currentDate,
                currentTick,
                activePlayerCountryTag,
                tickHandler,
                countries
            );
        }

        /// <summary>
        /// Evaluates a list of events for all countries.
        /// </summary>
        private void EvaluateEventList(
            IEnumerable<GameEvent> events,
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag,
            TickHandler tickHandler,
            IDictionary<string, Country> countries
        )
        {
            foreach (var evt in events)
            {
                if (evt.IsTriggeredOnly)
                    continue;

                if (evt is NewsEvent newsEvent)
                {
                    EvaluateNewsEvent(
                        newsEvent,
                        worldState,
                        currentDate,
                        currentTick,
                        activePlayerCountryTag,
                        tickHandler,
                        countries
                    );
                }
                else if (evt is CountryEvent countryEvent)
                {
                    EvaluateCountryEvent(
                        countryEvent,
                        worldState,
                        currentDate,
                        currentTick,
                        activePlayerCountryTag,
                        tickHandler,
                        countries
                    );
                }
            }
        }

        /// <summary>
        /// Evaluates a news event globally once, then delivers to recipient countries.
        /// </summary>
        private void EvaluateNewsEvent(
            NewsEvent evt,
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag,
            TickHandler tickHandler,
            IDictionary<string, Country> countries
        )
        {
            // Evaluate global triggers and MTTH once
            var contextCountryTag =
                activePlayerCountryTag ?? countries.Values.FirstOrDefault()?.Tag;

            if (contextCountryTag == null)
                return;

            var globalContext = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentDate = currentDate,
                CurrentCountryTag = contextCountryTag,
                TickHandler = tickHandler,
                EventManager = eventManager,
            };

            // Check if event fires globally
            if (!evt.ShouldFire(globalContext, currentTick))
                return;

            // Event fired - mark as triggered if needed
            if (evt.TriggerOnlyOnce)
                evt.HasTriggered = true;

            // Deliver to recipient countries
            bool shownToPlayer = false;

            foreach (var country in countries.Values)
            {
                var countryContext = new EventEvaluationContext
                {
                    WorldState = worldState,
                    CurrentDate = currentDate,
                    CurrentCountryTag = country.Tag,
                    TickHandler = tickHandler,
                    EventManager = eventManager,
                };

                // Check if this country should receive the event
                if (!evt.EvaluateRecipients(countryContext))
                    continue;

                bool isPlayerCountry = country.Tag == activePlayerCountryTag;

                // Skip if already shown to player
                if (shownToPlayer && isPlayerCountry)
                    continue;

                if (evt.Hidden || !isPlayerCountry)
                {
                    AutoExecuteEvent(evt, countryContext);
                }
                else
                {
                    ShowEventToPlayer(evt, countryContext);
                    shownToPlayer = true;
                }
            }
        }

        /// <summary>
        /// Evaluates a country event independently for each eligible country.
        /// </summary>
        private void EvaluateCountryEvent(
            CountryEvent evt,
            WorldState worldState,
            DateTime currentDate,
            long currentTick,
            string? activePlayerCountryTag,
            TickHandler tickHandler,
            IDictionary<string, Country> countries
        )
        {
            bool shownToPlayer = false;
            bool eventFiredForAnyCountry = false;

            foreach (var country in countries.Values)
            {
                var context = new EventEvaluationContext
                {
                    WorldState = worldState,
                    CurrentDate = currentDate,
                    CurrentCountryTag = country.Tag,
                    TickHandler = tickHandler,
                    EventManager = eventManager,
                };

                if (evt.ShouldFire(context, currentTick))
                {
                    eventFiredForAnyCountry = true;
                    bool isPlayerCountry = country.Tag == activePlayerCountryTag;

                    if (shownToPlayer)
                    {
                        HandleAICountryAfterPlayerShown(evt, context, isPlayerCountry);
                    }
                    else
                    {
                        shownToPlayer = HandleEventFiring(evt, context, isPlayerCountry);
                    }
                }
            }

            // Only mark as triggered after processing all countries
            if (eventFiredForAnyCountry && evt.TriggerOnlyOnce)
            {
                evt.HasTriggered = true;
            }
        }

        private static void HandleAICountryAfterPlayerShown(
            GameEvent evt,
            EventEvaluationContext context,
            bool isPlayerCountry
        )
        {
            // AI countries still auto-execute after player was shown
            if (!isPlayerCountry && !evt.Hidden && evt.Options.Count > 0)
            {
                evt.Options[0].Execute(context);
            }
        }

        private static bool HandleEventFiring(
            GameEvent evt,
            EventEvaluationContext context,
            bool isPlayerCountry
        )
        {
            if (evt.Hidden || !isPlayerCountry)
            {
                AutoExecuteEvent(evt, context);
                return false;
            }
            else
            {
                ShowEventToPlayer(evt, context);
                return true;
            }
        }

        private static void AutoExecuteEvent(GameEvent evt, EventEvaluationContext context)
        {
            if (evt.Options.Count > 0)
            {
                evt.Options[0].Execute(context);
            }
            evt.ResetMTTH();
        }

        private static void ShowEventToPlayer(GameEvent evt, EventEvaluationContext context)
        {
            TickHandler.TriggerEvent(evt, context);
        }

        /// <summary>
        /// Fires an event immediately for a specific country, bypassing trigger evaluation.
        /// </summary>
        public bool FireEventDebug(
            string eventId,
            string countryTag,
            WorldState worldState,
            DateTime currentDate,
            string? activePlayerCountryTag,
            TickHandler tickHandler
        )
        {
            var countries = worldState.GetCountryTable();
            if (countries == null || !countries.TryGetValue(countryTag, out var country))
                return false;

            GameEvent? evt = FindEvent(eventId);
            if (evt == null)
                return false;

            var context = new EventEvaluationContext
            {
                WorldState = worldState,
                CurrentDate = currentDate,
                CurrentCountryTag = countryTag,
                TickHandler = tickHandler,
                EventManager = eventManager,
            };

            bool isPlayerCountry = countryTag == activePlayerCountryTag;
            if (evt.Hidden || !isPlayerCountry)
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

            return true;
        }

        private GameEvent? FindEvent(string eventId)
        {
            return (GameEvent?)eventManager.GetCountryEvents().FirstOrDefault(e => e.Id == eventId)
                ?? (GameEvent?)eventManager.GetNewsEvents().FirstOrDefault(e => e.Id == eventId);
        }
    }
}
