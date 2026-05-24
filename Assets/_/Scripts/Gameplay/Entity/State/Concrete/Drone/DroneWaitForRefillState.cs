using _.Scripts.Gameplay.Entity.Concrete;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneWaitForRefillState : AbstractState<DroneEntity>
    {
        public override string Identity => "Drone Wait For Refill";

        public DroneWaitForRefillState(DroneEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            
        }
    }
}