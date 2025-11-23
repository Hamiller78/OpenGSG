using System;

// Provide compatibility types in the global 'GameLogic' namespace so other projects
// that `using GameLogic;` can resolve types from the migrated OpenGSGLibrary2 assembly.
using M = OpenGSGLibrary2.GameLogic;

namespace GameLogic
{
    public class TickEventArgs : M.TickEventArgs
    {
        public TickEventArgs(int t) : base(t) { }
    }

    public class TickHandler : M.TickHandler
    {
        // inherits behaviour from migrated TickHandler
    }

    public abstract class Player : M.Player { }

    public class PlayerManager : M.PlayerManager { }

    public class HumanPlayer : M.HumanPlayer { }

    public class AiPlayer : M.AiPlayer { }
}
