namespace OpenGSGLibrary.Events
{
    /// <summary>
    /// Interface for event effects - actions that occur when an event option is chosen.
    /// </summary>
    public interface IEventEffect
    {
        /// <summary>
        /// Executes this effect on the game state.
        /// </summary>
        /// <param name="context">Game state context.</param>
        void Execute(object context);
    }
}
