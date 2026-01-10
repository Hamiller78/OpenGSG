using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Manages all game events - loading, storing, and evaluating event triggers.
    /// </summary>
    public class EventManager
    {
        private readonly Dictionary<string, GameEvent> _allEvents = new();
        private readonly Dictionary<string, CountryEvent> _countryEvents = new();
        private readonly Dictionary<string, NewsEvent> _newsEvents = new();

        /// <summary>
        /// Registers an event with the manager.
        /// </summary>
        /// <param name="gameEvent">Event to register.</param>
        public void RegisterEvent(GameEvent gameEvent)
        {
            if (string.IsNullOrEmpty(gameEvent.Id))
            {
                throw new ArgumentException("Event must have a valid ID", nameof(gameEvent));
            }

            if (_allEvents.ContainsKey(gameEvent.Id))
            {
                throw new InvalidOperationException(
                    $"Event with ID '{gameEvent.Id}' already registered"
                );
            }

            _allEvents[gameEvent.Id] = gameEvent;

            // Store in type-specific collections for faster lookup
            if (gameEvent is CountryEvent countryEvent)
            {
                _countryEvents[gameEvent.Id] = countryEvent;
            }
            else if (gameEvent is NewsEvent newsEvent)
            {
                _newsEvents[gameEvent.Id] = newsEvent;
            }
        }

        /// <summary>
        /// Gets an event by its ID.
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <returns>The event, or null if not found.</returns>
        public GameEvent? GetEvent(string eventId)
        {
            _allEvents.TryGetValue(eventId, out var gameEvent);
            return gameEvent;
        }

        /// <summary>
        /// Gets all country events.
        /// </summary>
        public IEnumerable<CountryEvent> GetCountryEvents() => _countryEvents.Values;

        /// <summary>
        /// Gets all news events.
        /// </summary>
        public IEnumerable<NewsEvent> GetNewsEvents() => _newsEvents.Values;

        /// <summary>
        /// Gets all registered events.
        /// </summary>
        public IEnumerable<GameEvent> GetAllEvents() => _allEvents.Values;

        /// <summary>
        /// Evaluates all country events for a specific country and returns those that can fire.
        /// </summary>
        /// <param name="context">Evaluation context with current game state.</param>
        /// <returns>List of events that can fire.</returns>
        public List<CountryEvent> GetTriggeredCountryEvents(EventEvaluationContext context)
        {
            var triggered = new List<CountryEvent>();

            foreach (var evt in _countryEvents.Values)
            {
                if (evt.EvaluateTriggers(context))
                {
                    triggered.Add(evt);
                }
            }

            return triggered;
        }

        /// <summary>
        /// Evaluates all news events and returns those that can fire.
        /// </summary>
        /// <param name="context">Evaluation context with current game state.</param>
        /// <returns>List of news events that can fire.</returns>
        public List<NewsEvent> GetTriggeredNewsEvents(EventEvaluationContext context)
        {
            var triggered = new List<NewsEvent>();

            foreach (var evt in _newsEvents.Values)
            {
                if (evt.EvaluateTriggers(context))
                {
                    triggered.Add(evt);
                }
            }

            return triggered;
        }

        /// <summary>
        /// Fires an event by ID.
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <param name="context">Evaluation context.</param>
        /// <returns>True if event was found and fired.</returns>
        public bool FireEvent(string eventId, EventEvaluationContext context)
        {
            var evt = GetEvent(eventId);
            if (evt == null)
            {
                return false;
            }

            evt.Fire(context);
            return true;
        }

        /// <summary>
        /// Clears all registered events.
        /// </summary>
        public void Clear()
        {
            _allEvents.Clear();
            _countryEvents.Clear();
            _newsEvents.Clear();
        }

        /// <summary>
        /// Gets the total count of registered events.
        /// </summary>
        public int EventCount => _allEvents.Count;
    }
}
