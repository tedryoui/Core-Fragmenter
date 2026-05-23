using System;
using System.Collections.Generic;
using _.Scripts.Data.Concrete;
using _.Scripts.Gameplay.Entity.Concrete.Core;
using _.Scripts.Gameplay.Player;
using _.Scripts.Gameplay.Utility;
using _.Scripts.Scriptable_Objects.Concrete.Entities;
using _.Scripts.Services;
using _.Scripts.User_Interface;
using Core.Scripts.Helpers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class CoreEntity : MonoEntity, IDamageable
    {
#region VContainer

        [Inject] private ServiceLocator _serviceLocator;
        [Inject] private PlayerProfile  _playerProfile;

        [Inject]
        private void Configure(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            
            CoreData.Position = transform.position;
            CoreData.Rotation = transform.rotation;
        }

#endregion

#region Mono Entity Overrides

        public override string              Identity       => "Core";
        
        public override List<AbstractState> PossibleStates => new List<AbstractState>()
        {
            new CoreWeakState(this),
            new CoreRadiationState(this)
        };

#endregion

#region References

        [SerializeField] private WorldUtilityWindowViewModel _worldUtilityWindowViewModel;
        public WorldUtilityWindowViewModel WorldUtilityWindowViewModel => _worldUtilityWindowViewModel;

#endregion
        
#region Fields & Properties

        private DataService _dataService;
        private DataService DataService => _dataService ??= _serviceLocator.Get<DataService>();
        
        private ScriptableObjectService _scriptableObjectService;
        private ScriptableObjectService ScriptableObjectService => _scriptableObjectService ??= _serviceLocator.Get<ScriptableObjectService>();
        
        private CoreData _coreData;
        public  CoreData CoreData => _coreData ??= DataService.Get<CoreData>("Core");
        
        private PlayerData _playerData;
        public  PlayerData PlayerData => _playerData ??= DataService.Get<PlayerData>(_playerProfile.ID);

#endregion

#region IDamageable overrides

        private Action<int> _onDamageReceived;
        public event Action<int> OnDamageReceived
        {
            add => _onDamageReceived += value;
            remove => _onDamageReceived -= value;
        }

        public void ReceiveDamage(int value)
        {
            CoreData.HealthPoints -= value;
            _onDamageReceived?.Invoke(value);

            if (CoreData.HealthPoints == 0)
                _onDeath?.Invoke();
        }

#endregion

#region Fields & Properties

        private Action _onDeath;

        public event Action OnDeath
        {
            add => _onDeath += value;
            remove => _onDeath -= value;
        }

#endregion

        private void Awake()
        {
            _onDamageReceived = delegate { };
            _onDeath          = delegate { };

            _onDamageReceived += OnDamageReceivedInvoked;
            _onDeath          += OnDeathInvoked;
        }

        private void OnDestroy()
        {
            _onDamageReceived -= OnDamageReceivedInvoked;
            _onDeath          -= OnDeathInvoked;
        }

        private void OnDeathInvoked()
        {
            var drops = CoreData.DropPerDeath;
            
            DropResources(drops, 1);
        }

        private void OnDamageReceivedInvoked(int amount)
        {
            var drops = CoreData.DropPerDamage;
            
            DropResources(drops, amount);
        }

        private void DropResources(IReadOnlyCollection<CoreDataPreset.Drop> drops, int amount)
        {
            foreach (var drop in drops)
            {
                if (PlayerData.Resource.HasResource(drop.ResourceIdentity))
                {
                    var quantity = PlayerData.Resource.GetResourceQuantity(drop.ResourceIdentity);
                    var append   = drop.ResourceQuantity * amount;
                    
                    PlayerData.Resource.SetResourceQuantity(drop.ResourceIdentity, quantity + append);
                }
            } 
        }
    }
}