using _.Scripts.Gameplay.Entity.Concrete;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneReachTargetState : AbstractState<DroneEntity>
    {
        public override string Identity => "Drone Reach Target";

        public DroneReachTargetState(DroneEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log($"OnEnter.DroneReachTargetState");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}