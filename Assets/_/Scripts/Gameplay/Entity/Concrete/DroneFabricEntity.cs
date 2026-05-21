using System.Collections.Generic;
using System.Linq;
using _.Scripts.Data.Concrete;
using _.Scripts.Scriptable_Objects;
using _.Scripts.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class DroneFabricEntity : MonoEntity
    {
        [Inject] private ServiceLocator _serviceLocator;
        
        public override string              Identity       => _identity;
        public override List<AbstractState> PossibleStates { get; }
        
        [SerializeField] private string _identity;

        private DataService _dataService;
        public DataService DataService => _dataService ??= _serviceLocator.Get<DataService>();
        
        private DroneFabricData _droneFabricData;
        public DroneFabricData DroneFabricData => _droneFabricData ??= DataService.Get<DroneFabricData>(Identity);
        
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