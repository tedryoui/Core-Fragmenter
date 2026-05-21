using System.Collections.Generic;
using System.Linq;
using _.Scripts.Data.Concrete;
using _.Scripts.Gameplay.Entity.Concrete.Dron_Fabric;
using _.Scripts.Gameplay.Player;
using _.Scripts.Scriptable_Objects;
using _.Scripts.Services;
using _.Scripts.User_Interface;
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
                    Debug.Log($"Order {activeOrder.Identity} completed");
                    DroneFabricData.RemoveOrder(activeOrder.Identity);
                }
            }
        }
    }
}