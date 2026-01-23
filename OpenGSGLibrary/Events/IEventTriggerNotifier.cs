using OpenGSGLibrary.GameLogic;

namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Abstraction for notifying UI when events are triggered.
    /// Allows testability by avoiding static TickHandler dependency.
    /// </summary>
    public interface IEventTriggerNotifier
    {
        /// <summary>
        /// Notifies that an event should be shown to the player.
        /// </summary>
        void TriggerEvent(GameEvent evt, EventEvaluationContext context);
    }

    /// <summary>
    /// Production implementation that delegates to TickHandler static event.
    /// </summary>
    public class TickHandlerEventNotifier : IEventTriggerNotifier
    {
        public void TriggerEvent(GameEvent evt, EventEvaluationContext context)
        {
            TickHandler.TriggerEvent(evt, context);
        }
    }

    /// <summary>
    /// Test implementation that records triggered events for verification.
    /// </summary>
    public class TestEventNotifier : IEventTriggerNotifier
    {
        public List<(GameEvent Event, EventEvaluationContext Context)> TriggeredEvents { get; } =
            new();

        public void TriggerEvent(GameEvent evt, EventEvaluationContext context)
        {
            TriggeredEvents.Add((evt, context));
        }
    }
}
