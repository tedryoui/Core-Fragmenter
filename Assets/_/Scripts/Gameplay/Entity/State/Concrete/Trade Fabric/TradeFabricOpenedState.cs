using _.Scripts.Data.Concrete;
using _.Scripts.User_Interface;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.Concrete.Trade_Fabric
{
    public class TradeFabricOpenedState : AbstractState<TradeFabricEntity>
    {
        public override string Identity => "Trade Fabric Opened";

        private PlayerData _playerData;
        
        public TradeFabricOpenedState(TradeFabricEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Button, true);
            Entity.WorldUtilityWindowViewModel.ConfigureButton("Trade!", false);
            Entity.WorldUtilityWindowViewModel.ActionButtonClicked += TradeButtonClicked;
            
            _playerData = Entity.DataService.Get<PlayerData>(Entity.PlayerID);
        }

        private void TradeButtonClicked()
        {
            var window = Entity.UserInterfaceService.Get<TradingWindowViewModel>();
            
            Debug.Log("Clicked");
            
            window.Show();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (math.distance(_playerData.Position, Entity.transform.position) <= 3f)
            {
                if (!Entity.WorldUtilityWindowViewModel.IsVisible())
                {
                    Entity.WorldUtilityWindowViewModel.Show();
                    Entity.WorldUtilityWindowViewModel.SetButtonInteractable(true);
                }
            }
            else
            {
                if (Entity.WorldUtilityWindowViewModel.IsVisible())
                {
                    Entity.WorldUtilityWindowViewModel.Hide();
                    Entity.WorldUtilityWindowViewModel.SetButtonInteractable(false);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Button, false);
            Entity.WorldUtilityWindowViewModel.ConfigureButton("", false);
            Entity.WorldUtilityWindowViewModel.ActionButtonClicked -= TradeButtonClicked;
        }
    }
}