using WorldData;

namespace Orders
{
    public abstract class Order
    {
        private long startTick_;
        private long completeTick_;
        private OrderType type_;

        public long GetCompletionTick() => completeTick_;

        public abstract void FinalizeOrder(WorldState currentWorld);
    }
}
