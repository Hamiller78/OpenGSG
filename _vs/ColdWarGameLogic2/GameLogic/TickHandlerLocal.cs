using System;
using System.Collections.Generic;
using WorldData;

namespace GameLogic
{
    // Local TickHandler implementation used by ColdWarGameLogic2 during migration.
    public class TickHandler
    {
        public event EventHandler? TickDone;

        private WorldState? currentWorldState_;
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
                    TickDone += prov.OnTickDone;
                }
            }
        }

        public WorldState GetState() => currentWorldState_!;

        public void FinishTick()
        {
            currentTick_ += 1;
            TickDone?.Invoke(this, EventArgs.Empty);
        }

        public long GetCurrentTick() => currentTick_;
    }
}
