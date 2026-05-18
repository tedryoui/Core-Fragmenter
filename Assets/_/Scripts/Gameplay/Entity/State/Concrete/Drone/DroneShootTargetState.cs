using _.Scripts.Gameplay.Entity.Concrete;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneShootTargetState : AbstractState<DroneEntity>
    {
        
        public DroneShootTargetState(DroneEntity entity) : base(entity)
        {
        }
    }
}