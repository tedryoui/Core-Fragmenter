using _.Scripts.User_Interface;
using Sirenix.OdinInspector;
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
            
            ShowUI();
        }

        private void OnEntityReceivedAnyDamage(int i)
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
            {
                Entity.CoreData.DealDamageTime += Time.deltaTime;
                
                UpdateUI();
            }
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
            
            HideUI();
        }
        
        private void ShowUI()
        {
            Entity.WorldUtilityWindowViewModel.SetVisible(true);
        }

        private void UpdateUI()
        {
            {
                var totalSeconds     = Entity.CoreData.DealDamageDelay;
                var remainingSeconds = Entity.CoreData.DealDamageTime;

                if (remainingSeconds > 0)
                {
                    if (Entity.WorldUtilityWindowViewModel.IsBitDisabled(WorldUtilityWindowViewModel.UtilityBit.Timer))
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer,
                            true);

                    if (Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Timer))
                        Entity.WorldUtilityWindowViewModel.SetTimer(remainingSeconds, totalSeconds,
                            $"Eject in: {(int)remainingSeconds:00} seconds");
                }
                else
                {
                    if (Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Timer))
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer,
                            false);
                }
            }

            {
                var maxHealthPoints     = Entity.CoreData.MaxHealthPoints;
                var currentHealthPoints = Entity.CoreData.HealthPoints;

                if (currentHealthPoints > 0)
                {
                    if (Entity.WorldUtilityWindowViewModel.IsBitDisabled(WorldUtilityWindowViewModel.UtilityBit.Health))
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Health,
                            true);

                    if (Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Health))
                        Entity.WorldUtilityWindowViewModel.SetHealth(currentHealthPoints, maxHealthPoints, "HP: ");
                }
                else
                {
                    if (Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Health))
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Health,
                            false);
                }
            }
        }

        private void HideUI()
        {
            Entity.WorldUtilityWindowViewModel.SetVisible(false);
            Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer, false);
            Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Health, false);
        }
    }
}