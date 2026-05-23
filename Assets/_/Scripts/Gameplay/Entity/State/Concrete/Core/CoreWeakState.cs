using _.Scripts.User_Interface;
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

            ShowUI();
            
            DropRadiationTimeToZero();
        }

        private void OnEntityReceivedAnyDamage(int i)
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
            {
                Entity.CoreData.RebootTime += Time.deltaTime;

                UpdateUI();
            }
            else 
                Entity.SetState("Core Radiation");
        }

        public override void OnExit()
        {
            Entity.OnDamageReceived -=  OnEntityReceivedAnyDamage;

            HideUI();
        }

        private void ShowUI()
        {
            Entity.WorldUtilityWindowViewModel.SetVisible(true);
        }

        private void UpdateUI()
        {
            {
                var totalSeconds     = Entity.CoreData.RebootDuration;
                var remainingSeconds = Entity.CoreData.RebootTime;

                if (remainingSeconds > 0)
                {
                    if (Entity.WorldUtilityWindowViewModel.IsBitDisabled(WorldUtilityWindowViewModel.UtilityBit
                            .Loading))
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Loading,
                            true);

                    if (Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Loading))
                        Entity.WorldUtilityWindowViewModel.SetLoadingProgress(remainingSeconds / totalSeconds,
                            "Rebooting...");
                }
                else
                {
                    if (Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Loading))
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Loading,
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
            Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Loading, false);
            Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Health, false);
        }
    }
}