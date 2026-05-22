using System.Collections.Generic;
using System.Data;
using System.Linq;
using _.Scripts.Data.Concrete;
using _.Scripts.Events;
using _.Scripts.Gameplay.Entity.Concrete.Dron_Fabric;
using _.Scripts.Gameplay.Player;
using _.Scripts.Scriptable_Objects;
using _.Scripts.Services;
using _.Scripts.User_Interface;
using Core.Scripts.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class DroneFabricEntity : MonoEntity
    {
        [Inject] private ServiceLocator _serviceLocator;
        [Inject] private PlayerProfile  _playerProfile;
        
        public override string              Identity       => _identity;
        public override List<AbstractState> PossibleStates =>  new List<AbstractState>()
        {
            new DroneFabricIdleState(this)
        };
        
        [SerializeField] private string _identity;
        
        [SerializeField] private WorldUtilityWindowViewModel _worldUtilityWindowViewModel;
        public WorldUtilityWindowViewModel WorldUtilityWindowViewModel => _worldUtilityWindowViewModel;

        private DataService _dataService;
        public DataService DataService => _dataService ??= _serviceLocator.Get<DataService>();
        
        private DroneFabricData _droneFabricData;
        public DroneFabricData DroneFabricData => _droneFabricData ??= DataService.Get<DroneFabricData>(Identity);
        
        private PlayerData _playerData;
        public  PlayerData PlayerData => _playerData ??= DataService.Get<PlayerData>(_playerProfile.ID);
        
        [SerializeField, ReadOnly] private List<BlueprintScriptableObject> _blueprints;

        public List<BlueprintScriptableObject> Blueprints
        {
            get => _blueprints;
            set => _blueprints = value;
        }

        public override void Update()
        {
            base.Update();

            ProcessActiveBlueprints();
        }

        private void ProcessActiveBlueprints()
        {
            if (DroneFabricData.ActiveOrders.Count == 0)
                return;

            foreach (var activeOrder in DroneFabricData.ActiveOrders.ToArray())
            {
                if (activeOrder.IsCompleted())
                {
                    DroneFabricData.RemoveOrder(activeOrder.Identity);
                    Debug.Log($"Order {activeOrder.Identity} completed");
                    
                    var blueprint = Blueprints.Find(x => x.Identity.Equals(activeOrder.Identity));
                    
                    if (blueprint == null) 
                        throw new EvaluateException($"Blueprint {activeOrder.Identity} not found");
                    
                    foreach (var outputEvent in blueprint.OutputEvents)
                    {
                        var e = EventFabric.Build(outputEvent.EventType, outputEvent.Data.Select(x => x.Data).ToArray());
                        
                        EventFabric.AnonymousPublish(outputEvent.EventType, e);
                    }
                }
            }
        }

        public void ConfirmOrder(DroneFabricData.Order order)
        {
            if (DroneFabricData.HasOrder(order.Identity))
                return;
            if (DroneFabricData.ActiveOrders.Count >= DroneFabricData.MaxActiveOrders)
                return;
            if (!PlayerData.Unlock.HasBlueprint(order.Identity) || 
                !PlayerData.Unlock.IsUnlocked(order.Identity))
                throw new EvaluateException($"Order {order.Identity} not exists!");
                
            DroneFabricData.AddOrder(order);
            
        }
    }
}