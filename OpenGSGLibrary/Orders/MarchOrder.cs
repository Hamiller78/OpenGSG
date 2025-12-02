using WorldData;

namespace Orders
{
    /// <summary>
    /// Order that moves an army to a target province when finalized.
    /// </summary>
    public class MarchOrder : Order
    {
        private readonly Military.Army army_;
        private readonly int targetProvince_;

        public MarchOrder(Military.Army army, int targetProvince)
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
