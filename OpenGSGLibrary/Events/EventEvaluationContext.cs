using System;
using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.GameLogic;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Context object passed to event triggers and effects for evaluation.
    /// Contains all information needed to evaluate game state.
    /// </summary>
    public class EventEvaluationContext
    {
        /// <summary>
        /// Current world state.
        /// </summary>
        public WorldState? WorldState { get; set; }

        /// <summary>
        /// Current game date.
        /// </summary>
        public DateTime CurrentDate { get; set; }

        /// <summary>
        /// Tag of the country currently being evaluated (for country events).
        /// </summary>
        public string CurrentCountryTag { get; set; } = string.Empty;

        /// <summary>
        /// Reference to the tick handler (for triggering events, accessing game time, etc.).
        /// </summary>
        public TickHandler? TickHandler { get; set; }

        /// <summary>
        /// Reference to the event manager (for triggering other events via effects).
        /// </summary>
        public EventManager? EventManager { get; set; }
    }
}
