using System;
using OpenGSGLibrary.Events;

namespace OpenGSGLibrary.GameLogic
{
    /// <summary>
    /// Event args for when a game event is triggered.
    /// </summary>
    public class GameEventTriggeredEventArgs : EventArgs
    {
        public GameEvent Event { get; }
        public EventEvaluationContext Context { get; }

        public GameEventTriggeredEventArgs(GameEvent evt, EventEvaluationContext context)
        {
            Event = evt;
            Context = context;
        }
    }
}
