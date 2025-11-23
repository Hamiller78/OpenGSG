using System;
using System.Collections.Generic;
using WorldData;

namespace GameLogic
{
    // Minimal shim of GameLogic types so ColdWarGameLogic2 can compile during migration.
    public class TickEventArgs : EventArgs
    {
        public int tick { get; set; }
        public TickEventArgs(int t) { tick = t; }
    }

    public abstract class Player
    {
        private bool tickDone_ = false;
        public bool IsTickDone() => tickDone_;
    }

    public class PlayerManager
    {
        private static List<Player> playerList_ = new List<Player>();
        public void CalculateStrategies(WorldState currentWorldState) { /* no-op shim */ }
        public bool IsEverybodyDone()
        {
            foreach (var p in playerList_) if (!p.IsTickDone()) return false;
            return true;
        }
    }

    public class TickHandler
    {
        public event EventHandler<TickEventArgs>? TickDone;

        private PlayerManager playerManager_ = new PlayerManager();
        private WorldState? currentWorldState_ = null;
        private long currentTick_ = 0;

        public void ConnectProvinceEventHandlers(WorldState newState)
        {
            currentWorldState_ = newState;
            var provTable = currentWorldState_?.GetProvinceTable();
            if (provTable != null)
            {
                foreach (var prov in provTable.Values)
                {
                    // provinces expect OnTickDone(object, EventArgs)
                    TickDone += (s, e) => prov.OnTickDone(s, e);
                }
            }
        }

        public WorldState GetState() => currentWorldState_!;

        public void FinishTick()
        {
            currentTick_ += 1;
            TickDone?.Invoke(this, new TickEventArgs((int)currentTick_));
        }

        public long GetCurrentTick() => currentTick_;
    }
}
