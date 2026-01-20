using System.Linq;
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
        public bool IsNewsEvent { get; set; } // true for news_event, false for country_event

        public void Execute(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return;

            if (evalContext.TickHandler == null || evalContext.EventManager == null)
                return;

            // Find the event to trigger
            GameEvent? eventToFire = null;

            if (IsNewsEvent)
            {
                eventToFire = evalContext
                    .EventManager.GetNewsEvents()
                    .FirstOrDefault(e => e.Id == EventId);
            }
            else
            {
                eventToFire = evalContext
                    .EventManager.GetCountryEvents()
                    .FirstOrDefault(e => e.Id == EventId);
            }

            if (eventToFire == null)
                return;

            // Fire the event appropriately based on type and triggers
            if (IsNewsEvent && eventToFire.Triggers.Count > 0)
            {
                // News event with triggers - check each country
                var countries = evalContext.WorldState?.GetCountryTable();
                if (countries != null)
                {
                    foreach (var country in countries.Values)
                    {
                        var newsContext = new EventEvaluationContext
                        {
                            WorldState = evalContext.WorldState,
                            CurrentDate = evalContext.CurrentDate,
                            CurrentCountryTag = country.Tag,
                            TickHandler = evalContext.TickHandler,
                            EventManager = evalContext.EventManager,
                        };

                        // Check if triggers match for this country
                        if (eventToFire.EvaluateTriggers(newsContext))
                        {
                            bool isPlayerCountry =
                                country.Tag == evalContext.TickHandler.ActivePlayerCountryTag;

                            if (!isPlayerCountry)
                            {
                                // AI auto-executes first option
                                if (eventToFire.Options.Count > 0)
                                {
                                    eventToFire.Options[0].Execute(newsContext);
                                }
                            }
                            else
                            {
                                // Show to player - use helper method
                                TickHandler.TriggerEvent(eventToFire, newsContext);
                            }
                        }
                    }
                }
            }
            else if (IsNewsEvent)
            {
                // Global news event - show to active player only
                if (!string.IsNullOrEmpty(evalContext.TickHandler.ActivePlayerCountryTag))
                {
                    TickHandler.TriggerEvent(eventToFire, evalContext);
                }
            }
            else
            {
                // Country event - fire for current country context
                bool isPlayerCountry =
                    evalContext.CurrentCountryTag == evalContext.TickHandler.ActivePlayerCountryTag;

                if (!isPlayerCountry)
                {
                    // AI auto-executes
                    if (eventToFire.Options.Count > 0)
                    {
                        eventToFire.Options[0].Execute(evalContext);
                    }
                }
                else
                {
                    // Show to player - use helper method
                    TickHandler.TriggerEvent(eventToFire, evalContext);
                }
            }
        }
    }
}
