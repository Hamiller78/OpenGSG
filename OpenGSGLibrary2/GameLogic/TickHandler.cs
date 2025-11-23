using System;
using System.Collections.Generic;
using WorldData;

namespace GameLogic
{
    // Tiny local TickHandler shim for ColdWarGameLogic2
    public class TickHandler
    {
        public event EventHandler? TickDone;
        private WorldState? state_;
        private long currentTick_ = 0;

        public void ConnectProvinceEventHandlers(WorldState newState)
        {
            state_ = newState;
            var prov = state_?.GetProvinceTable();
            if (prov != null)
            {
                foreach (var p in prov.Values)
                {
                    TickDone += p.OnTickDone;
                }
            }
        }

        public WorldState GetState() => state_!;
        public void FinishTick()
        {
            currentTick_++;
            TickDone?.Invoke(this, EventArgs.Empty);
        }
        public long GetCurrentTick() => currentTick_;
    }
}
