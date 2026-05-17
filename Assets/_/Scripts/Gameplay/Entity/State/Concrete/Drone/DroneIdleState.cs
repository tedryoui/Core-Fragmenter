using _.Scripts.Gameplay.Entity.Concrete;
using UnityEngine;

namespace @_.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneIdleState : AbstractState<DroneEntity>
    {
        public override string Identity => "Drone Idle";

        public DroneIdleState(DroneEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log($"OnEnter.DroneIdleState");
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