using System;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Checks if the current country matches a specific tag.
    /// Used in event scoping (e.g., "tag = USA" means event is for USA).
    /// </summary>
    public class TagTrigger : IEventTrigger
    {
        public string Tag { get; set; } = string.Empty;

        public bool Evaluate(object context)
        {
            // Context should include information about which country is being evaluated
            // Implementation depends on EventEvaluationContext structure
            if (context is EventEvaluationContext evalContext)
            {
                return evalContext.CurrentCountryTag == Tag;
            }
            return false;
        }
    }

    /// <summary>
    /// Checks if a country with the given tag exists in the game.
    /// </summary>
    public class CountryExistsTrigger : IEventTrigger
    {
        public string Tag { get; set; } = string.Empty;

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            var countries = evalContext.WorldState?.GetCountryTable();
            return countries != null && countries.ContainsKey(Tag);
        }
    }

    /// <summary>
    /// Checks if the current game date is >= the specified date.
    /// </summary>
    public class DateGreaterEqualTrigger : IEventTrigger
    {
        public DateTime Date { get; set; }

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            return evalContext.CurrentDate >= Date;
        }
    }

    /// <summary>
    /// Checks if the current game date is < the specified date.
    /// </summary>
    public class DateLessThanTrigger : IEventTrigger
    {
        public DateTime Date { get; set; }

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            return evalContext.CurrentDate < Date;
        }
    }

    /// <summary>
    /// Scoped trigger - evaluates triggers in the context of a specific country.
    /// Example: "ROK = { has_war = no }" evaluates has_war trigger for ROK.
    /// </summary>
    public class CountryScopeTrigger : IEventTrigger
    {
        public string CountryTag { get; set; } = string.Empty;
        public List<IEventTrigger> ScopedTriggers { get; set; } = new List<IEventTrigger>();

        public bool Evaluate(object context)
        {
            if (context is not EventEvaluationContext evalContext)
                return false;

            // Create a new context with the scoped country as current
            var scopedContext = new EventEvaluationContext
            {
                WorldState = evalContext.WorldState,
                CurrentDate = evalContext.CurrentDate,
                CurrentCountryTag = CountryTag,
                TickHandler = evalContext.TickHandler,
            };

            foreach (var trigger in ScopedTriggers)
            {
                if (!trigger.Evaluate(scopedContext))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Context object passed to event triggers and effects for evaluation.
    /// Contains all information needed to evaluate game state.
    /// </summary>
    public class EventEvaluationContext
    {
        public WorldState? WorldState { get; set; }
        public DateTime CurrentDate { get; set; }
        public string CurrentCountryTag { get; set; } = string.Empty;
        public object? TickHandler { get; set; }
    }
}
