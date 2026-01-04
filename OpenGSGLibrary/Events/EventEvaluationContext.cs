using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.Events
{
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
