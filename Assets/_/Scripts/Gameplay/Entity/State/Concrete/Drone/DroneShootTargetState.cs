using _.Scripts.Gameplay.Entity.Concrete;
using _.Scripts.Gameplay.Utility;
using _.Scripts.Gameplay.Utility.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneShootTargetState : AbstractState<DroneEntity>
    {
        public override string Identity => "Drone Shoot Target";

        private MonoEntity  _targetEntity;
        private IDamageable _damageable;

        private quaternion TargetRotation => quaternion
            .LookRotation(
                (float3)_targetEntity.transform.position - Entity.DroneData.Position,
                new float3(0.0f, 1.0f, 0.0f)
            );
        
        public DroneShootTargetState(DroneEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            SetDroneDataValues();
            EnsureAgentHasProperValues();
            
            _targetEntity = Entity.WorldService.Entities[Entity.DroneData.TargetIdentity] as MonoEntity;
            if (!(_targetEntity is IDamageable damageable))
                Entity.SetState("Drone Idle");
            else 
                this._damageable = damageable;

            Entity.NavMeshAgent.Sleep();
            
        }
        
        private void EnsureAgentHasProperValues()
        {
            Entity.NavMeshAgent.angularSpeed = Entity.DroneData.CurrentAngularSpeed;
        }

        private void SetDroneDataValues()
        {
            Entity.DroneData.CurrentAngularSpeed = Entity.DroneData.AngularSpeed;
        }

        private void ResetDroneDataValues()
        {
            Entity.DroneData.CurrentAngularSpeed = 0.0f;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            AlignDroneRotation();
            WaitAndDealDamage();
        }

        private void AlignDroneRotation()
        {
            Entity.DroneData.Rotation = math.slerp(
                Entity.DroneData.Rotation,
                TargetRotation,
                4.0f * Time.deltaTime
            );
        }

        private void WaitAndDealDamage()
        {
            if (math.angle(Entity.DroneData.Rotation, TargetRotation) >= math.radians(5))
            {
                Entity.DroneData.DealDamageTime = 0.0f;
                return;
            }
            
            if (Entity.DroneData.DealDamageTime >= Entity.DroneData.DealDamageDelay)
            {
                _damageable.ReceiveDamage(1);
                Entity.DroneData.DealDamageTime = 0.0f;
            }
            else 
                Entity.DroneData.DealDamageTime += Time.deltaTime;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            ResetDroneDataValues();
            EnsureAgentHasProperValues();

            Entity.NavMeshAgent.WakeUp();
        }
    }
}