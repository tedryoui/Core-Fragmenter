using UnityEngine;

namespace _.Scripts.Gameplay.Entity.Concrete.Core
{
    public class CoreRadiationState : AbstractState<CoreEntity>
    {
        public override string Identity => "Core Radiation";

        public CoreRadiationState(CoreEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            Entity.OnDamageReceived += OnEntityReceivedAnyDamage;
        }

        private void OnEntityReceivedAnyDamage()
        {
            Entity.SetState("Core Weak");
            DropDealDamageTimeToZero();
        }

        private void DropDealDamageTimeToZero()
        {
            Entity.CoreData.DealDamageTime = 0.0f;
        }

        public override void OnUpdate()
        {
            if (Entity.CoreData.DealDamageTime < Entity.CoreData.DealDamageDelay)
                Entity.CoreData.DealDamageTime += Time.deltaTime;
            else
                DealDamage();
        }

        private void DealDamage()
        {
            Entity.CoreData.DealDamageTime = 0.0f;
        }

        public override void OnExit()
        {
            Entity.OnDamageReceived -= OnEntityReceivedAnyDamage;
        }
    }
}