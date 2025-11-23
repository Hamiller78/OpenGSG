using System;
using System.Collections.Generic;
using WorldData;

namespace GameLogic
{
    // Lightweight TickHandler used by ColdWarGameLogic2 to avoid cross-project missing type errors during migration.
    public class TickHandler
    {
        private long currentTick_ = 0;
        public void ConnectProvinceEventHandlers(WorldState newState)
        {
            // subscribe provinces if they implement OnTickDone
            if (newState?.GetProvinceTable() is IDictionary<int, Province> provTable)
            {
                // no-op for now; provinces handle events directly in migrated code
            }
        }

        public void FinishTick()
        {
            currentTick_ += 1;
            // no event dispatch in this shim
        }

        public long GetCurrentTick() => currentTick_;

        public WorldState GetState() => throw new NotImplementedException();
    }
}
