using System.Linq;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Effect that triggers another event (country or news event).
    /// Used in hidden_effect blocks to chain events.
    /// </summary>
    public class TriggerEventEffect : IEventEffect
    {
        public string EventId { get; set; } = string.Empty;
        public bool IsNewsEvent { get; set; }

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            if (
                evalContext.TickHandler == null
                || evalContext.EventManager == null
                || evalContext.WorldState == null
            )
                return;

            // Find the event to trigger
            GameEvent? eventToFire = FindEvent(evalContext.EventManager);
            if (eventToFire == null)
                return;

            var countries = evalContext.WorldState.GetCountryTable();
            if (countries == null)
                return;

            EvaluateEventForAllCountries(eventToFire, evalContext, countries);
        }

        private GameEvent? FindEvent(EventManager eventManager)
        {
            return IsNewsEvent
                ? eventManager.GetNewsEvents().FirstOrDefault(e => e.Id == EventId)
                : (GameEvent?)eventManager.GetCountryEvents().FirstOrDefault(e => e.Id == EventId);
        }

        private void EvaluateEventForAllCountries(
            GameEvent eventToFire,
            EventEvaluationContext evalContext,
            IDictionary<string, Country> countries
        )
        {
            if (eventToFire is NewsEvent newsEvent)
            {
                EvaluateNewsEvent(newsEvent, evalContext, countries);
            }
            else if (eventToFire is CountryEvent countryEvent)
            {
                EvaluateCountryEvent(countryEvent, evalContext, countries);
            }
        }

        private void EvaluateNewsEvent(
            NewsEvent evt,
            EventEvaluationContext evalContext,
            IDictionary<string, Country> countries
        )
        {
            // Evaluate triggers globally once (using triggering country's context)
            if (!evt.EvaluateTriggers(evalContext))
                return;

            // Deliver to recipient countries
            bool shownToPlayer = false;

            foreach (var country in countries.Values)
            {
                var countryContext = new EventEvaluationContext
                {
                    WorldState = evalContext.WorldState,
                    CurrentDate = evalContext.CurrentDate,
                    CurrentCountryTag = country.Tag,
                    TickHandler = evalContext.TickHandler,
                    EventManager = evalContext.EventManager,
                };

                // Check if this country should receive the event
                if (!evt.EvaluateRecipients(countryContext))
                    continue;

                bool isPlayerCountry =
                    country.Tag == evalContext.TickHandler!.ActivePlayerCountryTag;

                // Skip if already shown to player
                if (shownToPlayer && isPlayerCountry)
                    continue;

                if (ShouldAutoExecute(evt, isPlayerCountry))
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

        private void EvaluateCountryEvent(
            CountryEvent evt,
            EventEvaluationContext evalContext,
            IDictionary<string, Country> countries
        )
        {
            bool shownToPlayer = false;

            foreach (var country in countries.Values)
            {
                var countryContext = new EventEvaluationContext
                {
                    WorldState = evalContext.WorldState,
                    CurrentDate = evalContext.CurrentDate,
                    CurrentCountryTag = country.Tag,
                    TickHandler = evalContext.TickHandler,
                    EventManager = evalContext.EventManager,
                };

                if (evt.EvaluateTriggers(countryContext))
                {
                    bool isPlayerCountry =
                        country.Tag == evalContext.TickHandler!.ActivePlayerCountryTag;

                    // Skip if already shown to player
                    if (shownToPlayer && isPlayerCountry)
                        continue;

                    if (ShouldAutoExecute(evt, isPlayerCountry))
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
        }

        private static bool ShouldAutoExecute(GameEvent evt, bool isPlayerCountry)
        {
            return evt.Hidden || !isPlayerCountry;
        }

        private static void AutoExecuteEvent(GameEvent evt, EventEvaluationContext context)
        {
            if (evt.Options.Count > 0)
            {
                evt.Options[0].Execute(context);
            }
        }

        private static void ShowEventToPlayer(GameEvent evt, EventEvaluationContext context)
        {
            TickHandler.TriggerEvent(evt, context);
        }
    }
}
