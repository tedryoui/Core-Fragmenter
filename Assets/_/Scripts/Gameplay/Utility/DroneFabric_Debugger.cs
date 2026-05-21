using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Data.Concrete;
using _.Scripts.Gameplay.Entity.Concrete;
using _.Scripts.Gameplay.Player;
using _.Scripts.Scriptable_Objects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Gameplay.Utility
{
    public class DroneFabric_Debugger : MonoBehaviour
    {
        [SerializeField, ReadOnly] private DroneFabricEntity _entity;

        private void OnValidate()
        {
            if (_entity == null)
                _entity = GetComponent<DroneFabricEntity>();
        }
        
        [BoxGroup("All Blueprints")]
        [InlineProperty] [SerializeField]
        private List<string> _allBlueprintIdentities;
        
        [BoxGroup("Available Blueprints")]
        [InlineProperty] [SerializeField]
        private List<string> _availableBlueprintIdentities;
        
        [BoxGroup("Unlocked Blueprints")]
        [InlineProperty] [SerializeField]
        private List<string> _unlockedBlueprintIdentities;
        
        [BoxGroup("Active Orders")]
        [InlineProperty] [SerializeField]
        private List<string> _activeOrderIdentities;

        [BoxGroup("Confirm Order")] 
        [SerializeField] private string _identity;
        
        [BoxGroup("Confirm Order")]
        [Button]
        private void ConfirmOrder()
        {
            var blueprint = _entity.Blueprints.FirstOrDefault(x => x.Identity.Equals(_identity));

            if (blueprint != null)
            {
                var identity = _identity;
                var start = DateTime.Now;
                var end = DateTime.Now + TimeSpan.FromSeconds(blueprint.Process switch
                {
                    BlueprintScriptableObject.BlueprintProcess.Delayed => blueprint.BlueprintDuration,
                    BlueprintScriptableObject.BlueprintProcess.Instant => 0,
                    _                                                  => throw new ArgumentOutOfRangeException()
                });

                _entity.ConfirmOrder(new DroneFabricData.Order
                {
                    Identity = identity,
                    Start    = start,
                    End      = end
                });
            }
        }

        private void Update()
        {
            _allBlueprintIdentities = _entity.Blueprints.Select(x => x.Identity).ToList();
            
            _activeOrderIdentities = _entity.DroneFabricData.ActiveOrders.Select(x => x.Identity).ToList();
            
            _availableBlueprintIdentities = _entity.Blueprints.Select(x => x.Identity).Where(x => _entity.PlayerData.Unlock.HasBlueprint(x)).ToList();
            
            _unlockedBlueprintIdentities = _entity.Blueprints.Select(x => x.Identity).Where(x => _entity.PlayerData.Unlock.IsUnlocked(x)).ToList();
        }
    }
}