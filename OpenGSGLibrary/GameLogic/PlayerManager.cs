using OpenGSGLibrary.GameDataManager;
using System;
using System.Collections.Generic;

namespace OpenGSGLibrary.GameLogic
{
    public class PlayerManager
    {
        private static List<Player> playerList_ = new List<Player>();

        public void CalculateStrategies(WorldState currentWorldState)
        {
            throw new NotImplementedException();
        }

        public bool IsEverybodyDone()
        {
            foreach (var currentPlayer in playerList_)
            {
                if (!currentPlayer.IsTickDone())
                    return false;
            }

            return true;
        }
    }
}
