using System;
using System.Collections.Generic;
using _.Scripts.Data.Concrete;
using _.Scripts.Gameplay.Entity.Concrete.Core;
using _.Scripts.Gameplay.Utility;
using _.Scripts.Scriptable_Objects.Concrete.Entities;
using _.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class CoreEntity : MonoEntity, IDamageable
    {
#region VContainer

        [Inject] private ServiceLocator _serviceLocator;

        [Inject]
        private void Configure(ServiceLocator serviceLocator)
        {
            Debug.Log("Injected!");
            
            _serviceLocator = serviceLocator;
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

#region Fields & Properties

        private DataService _dataService;
        private DataService DataService => _dataService ??= _serviceLocator.Get<DataService>();
        
        private ScriptableObjectService _scriptableObjectService;
        private ScriptableObjectService ScriptableObjectService => _scriptableObjectService ??= _serviceLocator.Get<ScriptableObjectService>();
        
        private CoreData _coreData;
        public  CoreData CoreData => _coreData ??= DataService.Get<CoreData>("Core");

#endregion

#region IDamageable overrides

        private Action _onDamageReceived;
        public event Action OnDamageReceived
        {
            add => _onDamageReceived += value;
            remove => _onDamageReceived -= value;
        }

        public void ReceiveDamage(int value)
        {
            CoreData.HealthPoints -= value;
            _onDamageReceived?.Invoke();
        }

#endregion

        private void Awake()
        {
            _onDamageReceived = delegate { };
        }
    }
}