using WorldData;

namespace OpenGSGLibrary2.GameLogic
{
    /// <summary>
    /// Base class for players.
    /// Translated from original VB implementation.
    /// </summary>
    public abstract class Player
    {
        private bool tickDone_ = false;

        private int playerIndex_;
        private Country country_;

        /// <summary>
        /// Flag whether player is done for the turn.
        /// For human players can be set to false to pause (e.g. when an event pops up)
        /// </summary>
        public bool IsTickDone() => tickDone_;
    }
}
