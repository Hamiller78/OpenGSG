using System;
using OpenGSGLibrary.GameDataManager;

namespace OpenGSGLibrary.GameLogic
{
    /// <summary>
    /// Migrated TickHandler from the original VB implementation.
    /// Responsible for keeping the current world state and notifying
    /// subscribed provinces when a tick finishes.
    /// </summary>
    public class TickHandler
    {
        /// <summary>
        /// Notifies subscribers (provinces and other systems) that the tick finished.
        /// Provinces are subscribed to this to run their per-tick updates.
        /// </summary>
        public event EventHandler? TickDone;

        /// <summary>
        /// Raised after all TickDone subscribers have been invoked.
        /// Intended for the UI layer (or adapters) to perform a single refresh
        /// after the model update is complete.
        /// Static so UI adapters can wire up easily without holding a reference to a TickHandler instance.
        /// </summary>
        public static event EventHandler<TickEventArgs>? UIRefreshRequested;

        private PlayerManager playerManager_ = new PlayerManager();
        private WorldState? currentWorldState_;
        private long currentTick_ = 0;

        /// <summary>
        /// Connects world state to tick handler which includes setting the state in TickHandlers
        /// and associating event handlers.
        /// </summary>
        /// <param name="newState">WorldState to set in TickHandler.</param>
        public void ConnectProvinceEventHandlers(WorldState newState)
        {
            currentWorldState_ = newState;

            var provinceDict = currentWorldState_?.GetProvinceTable();
            if (provinceDict != null)
            {
                foreach (var province in provinceDict.Values)
                {
                    // Provinces implement OnTickDone(object, EventArgs)
                    // The TickDone event uses EventArgs; subscribe via a lambda to adapt the signature.
                    TickDone += (s, ev) => province.OnTickDone(s, ev);
                }
            }
        }

        /// <summary>
        /// Returns the world state used by the tick handler
        /// </summary>
        /// <returns>WorldState object for the current tick which will be modified each tick.</returns>
        public WorldState GetState() => currentWorldState_!;

        /// <summary>
        /// Method to do stuff when an new tick starts.
        /// In the original code this collected GUI input and launched AI threads.
        /// </summary>
        public void BeginNewTick()
        {
            // do timed game events
            // launch AI threads
            playerManager_.CalculateStrategies(currentWorldState_);
            // collect GUI input
        }

        /// <summary>
        /// Is the current tick complete?
        /// </summary>
        /// <returns>Boolean whether the current tick is complete</returns>
        public bool IsTickComplete()
        {
            if (playerManager_.IsEverybodyDone() == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to do stuff when a tick is completed and the next WorldState is calculated.
        /// Notifies provinces (TickDone) and then requests a single UI refresh (UIRefreshRequested).
        /// </summary>
        public void FinishTick()
        {
            // lock GUI input
            // calculate world in next tick
            currentTick_ += 1;
            var args = new TickEventArgs((int)currentTick_);

            // notify all interested classes (synchronous invoke)
            TickDone?.Invoke(this, args);

            // After provinces and other TickDone subscribers have run, request UI refresh
            // so the UI can pull latest model state (or ViewModels can Refresh()).
            UIRefreshRequested?.Invoke(this, args);
        }

        /// <summary>
        /// Gets the current tick number.
        /// Conversion to a date is the responsibility of the game-specific logic.
        /// </summary>
        /// <returns>Current tick as a long integer.</returns>
        public long GetCurrentTick() => currentTick_;
    }

    /// <summary>
    /// Helper class passed with the TickDone event.
    /// Extra argument is the current tick number.
    /// </summary>
    public class TickEventArgs : EventArgs
    {
        public int tick { get; set; }

        public TickEventArgs(int tickPar)
        {
            tick = tickPar;
        }
    }
}
