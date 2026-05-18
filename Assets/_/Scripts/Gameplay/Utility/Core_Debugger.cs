using System;
using _.Scripts.Gameplay.Entity.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Gameplay.Utility
{
    public class Core_Debugger : MonoBehaviour
    {
        [SerializeField, DisableIf("@true")] private CoreEntity _entity;

        [HideInEditorMode] [ShowInInspector] private string _currentStateIdentity;

        [HideInEditorMode] [ShowInInspector] private int _maxHealthPoints;
        [HideInEditorMode] 
        [ProgressBar(min: 0, maxGetter: "@this._entity.CoreData.MaxHealthPoints")] 
        [ShowInInspector] private int _currentHealthPoints;

        [HideInEditorMode]
        [ProgressBar(min: 0.0f, maxGetter: "@this._entity.CoreData.RebootDuration")]
        [ShowInInspector] private float _rebootTime;

        [HideInEditorMode]
        [ProgressBar(min: 0.0f, maxGetter: "@this._entity.CoreData.DealDamageDelay")]
        [ShowInInspector] private float _dealDamageTime;

        [HideInEditorMode]
        [Button]
        private void TakeDamage(int value = 1)
        {
            _entity.ReceiveDamage(value);
        }

        private void OnValidate()
        {
            if (_entity == null)
                _entity = GetComponent<CoreEntity>();
        }

        private void Update()
        {
            _currentStateIdentity = _entity.CurrentState.Identity;
            
            _maxHealthPoints = _entity.CoreData.MaxHealthPoints;
            _currentHealthPoints = _entity.CoreData.HealthPoints;

            _rebootTime = _entity.CoreData.RebootTime;
            
            _dealDamageTime = _entity.CoreData.DealDamageTime;
        }
    }
}