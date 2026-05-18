using PrimeTween;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.Concrete.Core
{
    public class CoreWeakState : AbstractState<CoreEntity>
    {
        public override string Identity => "Core Weak";

        public CoreWeakState(CoreEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            Entity.OnDamageReceived += OnEntityReceivedAnyDamage;
            
            DropRadiationTimeToZero();
        }

        private void OnEntityReceivedAnyDamage()
        {
            DropRadiationTimeToZero();
        }

        private void DropRadiationTimeToZero()
        {
            Entity.CoreData.RebootTime = 0.0f;
        }

        public override void OnUpdate()
        {
            if (Entity.CoreData.RebootTime < Entity.CoreData.RebootDuration)
                Entity.CoreData.RebootTime += Time.deltaTime;
            else 
                Entity.SetState("Core Radiation");
        }

        public override void OnExit()
        {
            Entity.OnDamageReceived -=  OnEntityReceivedAnyDamage;
        }
    }
}