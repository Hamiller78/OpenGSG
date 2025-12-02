using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.Military;

namespace OpenGSGLibrary.Orders
{
    /// <summary>
    /// Order that moves an army to a target province when finalized.
    /// </summary>
    public class MarchOrder : Order
    {
        private readonly Army army_;
        private readonly int targetProvince_;

        public MarchOrder(Army army, int targetProvince)
        {
            army_ = army;
            targetProvince_ = targetProvince;
        }

        public override void FinalizeOrder(WorldState currentWorld)
        {
            var armyManager = currentWorld.GetArmyManager();
            armyManager?.MoveArmy(army_, targetProvince_);
        }
    }
}
