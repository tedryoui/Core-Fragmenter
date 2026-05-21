using System.Linq;
using _.Scripts.User_Interface;
using Unity.Mathematics;

namespace _.Scripts.Gameplay.Entity.Concrete.Dron_Fabric
{
    public class DroneFabricIdleState : AbstractState<DroneFabricEntity>
    {
        public override string Identity => "Drone Fabric Idle";

        public DroneFabricIdleState(DroneFabricEntity entity) : base(entity)
        {
        }

        private         bool   IsUIActive => Entity.WorldUtilityWindowViewModel.IsVisible();

        public override void OnEnter()
        {
            SetupUI();
            
            base.OnEnter();
        }

        private void SetupUI()
        {
            Entity.WorldUtilityWindowViewModel.ConfigureButton("Manage!", false);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (DoesPlayerInRadius())
            {
                if (!IsUIActive)
                    ShowUI();
                UpdateUI();
            }
            else
            {
                if (IsUIActive)
                    HideUI();
            }
        }

        private bool DoesPlayerInRadius()
        {
            var distance = math.distance(Entity.transform.position, Entity.PlayerData.Position);
            
            return distance <= 3.0f;
        }

        private void UpdateUI()
        {
            var isButtonActive =
                Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Button);
            var isOutOfOrderLimit = Entity.DroneFabricData.ActiveOrders.Count >= Entity.DroneFabricData.MaxActiveOrders;
            
            if (isButtonActive && isOutOfOrderLimit)
            {
                Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Button, false);
                Entity.WorldUtilityWindowViewModel.SetButtonInteractable(false);
            }
            else if (!isButtonActive && !isOutOfOrderLimit)
            {
                Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Button, true);
                Entity.WorldUtilityWindowViewModel.SetButtonInteractable(true);
            }

            var isTimerActive = Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Timer);
            var hasOrder = Entity.DroneFabricData.ActiveOrders.Count != 0;
            
            if (isTimerActive && !hasOrder)
                Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer, false);
            else if (!isTimerActive && hasOrder)
                Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer, true);
            
            isTimerActive = Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Timer);
            
            if (isTimerActive)
            {
                var first = Entity.DroneFabricData.ActiveOrders.First();
                Entity.WorldUtilityWindowViewModel.SetTimer(first.RemainingSeconds, first.TotalSeconds, "Processing!");
            }
        }

        private void ShowUI()
        {
            Entity.WorldUtilityWindowViewModel.SetVisible(true);
        }
        
        private void HideUI()
        {
            Entity.WorldUtilityWindowViewModel.SetVisible(false);
        }
    }
}