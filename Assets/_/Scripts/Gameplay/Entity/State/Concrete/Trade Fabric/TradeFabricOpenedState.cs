using System;
using System.Linq;
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
        
        private TradeEntityData.TradeOrder _tempOrder;
        
        public TradeFabricOpenedState(TradeFabricEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Entity.WorldUtilityWindowViewModel.ConfigureButton("Trade!", false);
            Entity.WorldUtilityWindowViewModel.ActionButtonClicked += TradeButtonClicked;
            
            _playerData = Entity.DataService.Get<PlayerData>(Entity.PlayerID);
        }

        private void TradeButtonClicked()
        {
            _tempOrder = new TradeEntityData.TradeOrder();
            
            var window = Entity.UserInterfaceService.Get<TradingWindowViewModel>();
            window.SetCatalog(Entity.TradeList);
            window.GetOrder          += () => _tempOrder;
            window.GetOrderSummary   += GetOrderSummary;
            window.OnTradeAdded      += OnTradeAdded;
            window.OnTradeRemoved    += OnTradeRemoved;
            window.OnClearCart       += OnClearCart;
            window.OnQuantityChanged += OnQuantityChanged;
            window.OrderConfirmed    += OrderConfirmed;
            
            window.Show();
        }

        private TradeOrderSummary GetOrderSummary()
        {
            if (!_tempOrder.HasItems)
                return new TradeOrderSummary(0, 0, 10);
            
            var totalBaseCost = _tempOrder.Lines.Sum(x =>
            {
                var tradeConfig = Entity.TradeList.FirstOrDefault(y => y.Identity.Equals(x.Identity));
                if (tradeConfig == null) return 0;
                var baseCost    = tradeConfig.BaseCost;

                return baseCost * x.Quantity;
            });
            var totalDeliveryTime = _tempOrder.Lines.Max(x =>
            {
                var tradeConfig = Entity.TradeList.FirstOrDefault(y => y.Identity.Equals(x.Identity));
                if (tradeConfig == null) return 0;
                return tradeConfig.DeliveryTime;
            });
            var deliverySurcharge = 10.0;
            
            return new TradeOrderSummary(totalBaseCost, totalDeliveryTime, deliverySurcharge);
        }

        private void OnTradeAdded(string identity)
        {
            _tempOrder.AddLine(identity);
        }

        private void OnTradeRemoved(string identity)
        {
            _tempOrder.RemoveLine(identity);
        }

        private void OnClearCart()
        {
            _tempOrder.Clear();
        }

        private void OnQuantityChanged(string identity, int quantity)
        {
            _tempOrder.SetLine(identity, quantity);
        }

        private void OrderConfirmed()
        {
            var window = Entity.UserInterfaceService.Get<TradingWindowViewModel>();
            window.Hide();
            
            var summary   = GetOrderSummary();
            var whenCompletes = DateTime.Now + TimeSpan.FromSeconds(summary.TotalDeliveryTime);
            
            Entity.TradeEntityData.AddOrder(_tempOrder, whenCompletes);
            
            _tempOrder.Clear();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (math.distance(_playerData.Position, Entity.transform.position) <= 3f)
            {
                if (!Entity.WorldUtilityWindowViewModel.IsVisible())
                {
                    Entity.WorldUtilityWindowViewModel.Show();
                    
                    if (Entity.TradeEntityData.Orders.Count < Entity.TradeEntityData.MaxOrderQuantity &&
                        Entity.WorldUtilityWindowViewModel.IsBitDisabled(WorldUtilityWindowViewModel.UtilityBit.Button))
                    {
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Button, true);
                        Entity.WorldUtilityWindowViewModel.SetButtonInteractable(true);
                    }
                    if (Entity.TradeEntityData.Orders.Count > 0 &&
                        Entity.WorldUtilityWindowViewModel.IsBitDisabled(WorldUtilityWindowViewModel.UtilityBit.Timer))
                    {
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer, true);
                    }
                }
                else
                {
                    if (Entity.TradeEntityData.Orders.Count >= Entity.TradeEntityData.MaxOrderQuantity &&
                        Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Button))
                    {
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Button, false);
                        Entity.WorldUtilityWindowViewModel.SetButtonInteractable(false);
                    }
                    if (Entity.TradeEntityData.Orders.Count == 0 &&
                        Entity.WorldUtilityWindowViewModel.IsBitEnabled(WorldUtilityWindowViewModel.UtilityBit.Timer))
                    {
                        Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer, false);
                    }
                    
                    if (Entity.TradeEntityData.Orders.Count > 0)
                        UpdateWorldUtilityTimers();
                }
            }
            else
            {
                if (Entity.WorldUtilityWindowViewModel.IsVisible())
                {
                    Entity.WorldUtilityWindowViewModel.Hide();
                    Entity.WorldUtilityWindowViewModel.SetButtonInteractable(false);
                    
                    Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Button, false);
                    Entity.WorldUtilityWindowViewModel.SetBitVisible(WorldUtilityWindowViewModel.UtilityBit.Timer, false);
                }
            }
        }

        private void UpdateWorldUtilityTimers()
        {
            var firstOrder    = Entity.TradeEntityData.Orders.First();
            var totalSeconds  = firstOrder.TotalSeconds;
            var secondsRemain = firstOrder.SecondsRemain;
            var title         = $"Order #66";
            
            Entity.WorldUtilityWindowViewModel.SetTimer(secondsRemain, totalSeconds, title);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Entity.WorldUtilityWindowViewModel.ConfigureButton("", false);
            Entity.WorldUtilityWindowViewModel.ActionButtonClicked -= TradeButtonClicked;
        }
    }
}