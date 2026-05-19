namespace _.Scripts.Gameplay.Entity.Concrete.Trade_Fabric
{
    public class TradeFabricIdleState : AbstractState<TradeFabricEntity>
    {
        public override string Identity => "Trade Fabric Idle";

        public TradeFabricIdleState(TradeFabricEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Entity.SetState("Trade Fabric Opened");
        }
    }
}