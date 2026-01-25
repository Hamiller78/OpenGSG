using OpenGSGLibrary.GameDataManager;
using OpenGSGLibrary.Military;

namespace OpenGSGLibrary.Orders
{
    /// <summary>
    /// Order that moves a military formation to a target province when finalized.
    /// </summary>
    public class MarchOrder : Order
    {
        private readonly MilitaryFormation _formation;
        private readonly int _targetProvince;

        public MarchOrder(MilitaryFormation formation, int targetProvince)
        {
            _formation = formation;
            _targetProvince = targetProvince;
        }

        public override void FinalizeOrder(WorldState currentWorld)
        {
            // Simply update the formation's location
            // The formation is already referenced in the country's Military object
            _formation.Location = _targetProvince;
        }
    }
}
