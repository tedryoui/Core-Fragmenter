using System;
using System.Collections.Generic;
using _.Scripts.Events;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace _.Scripts.Data.Concrete
{
    public class CoreData : IData<CoreDataPreset>
    {
#region IData overrides

        public string Identity => "Core";
        public bool   IsSigned => true;

        public IData<CoreDataPreset> Fill(CoreDataPreset presetData)
        {
            _maxHealthPoints = presetData.HealthPoints;
            _rebootDuration  = presetData.RebootDuration;
            _dealDamageDelay = presetData.DealDamageDelay;
            
            _dropPerDamage = presetData.DropPerDamage;
            _dropPerDeath  = presetData.DropPerDeath;
            
            return this;
        }
        
#endregion
        
#region Fields & Properties

        private float3 _position;
        public  float3 Position
        {
            get => _position;
            set => _position = value;
        }
        
        private quaternion _rotation;
        public quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        private int _maxHealthPoints;
        public int MaxHealthPoints => _maxHealthPoints;

        private int _healthPoints;
        public int HealthPoints
        {
            get => _healthPoints;
            set
            {
                _healthPoints = value switch
                {
                    var nextValue when nextValue < 0 => 0,
                    var nextValue when nextValue > _maxHealthPoints => _maxHealthPoints,
                    _ => value
                };
            }
        }

        private float _rebootDuration;
        public  float RebootDuration => _rebootDuration;

        private float _rebootTime;
        public  float RebootTime
        {
            get => _rebootTime;
            set
            {
                _rebootTime = value switch
                {
                    var nextValue when nextValue < 0              => 0.0f,
                    var nextValue when nextValue > RebootDuration => RebootDuration,
                    _                                             => value
                };
            }
        }

        private float _dealDamageDelay;
        public   float DealDamageDelay => _dealDamageDelay;
        
        private float _dealDamageTime;
        public  float DealDamageTime
        {
            get => _dealDamageTime;
            set
            {
                _dealDamageTime = value switch
                {
                    var nextValue when nextValue < 0                => 0,
                    var nextValue when nextValue > _dealDamageDelay => _dealDamageDelay,
                    _                                               => value
                };
            }
        }

        private List<CoreDataPreset.Drop>                _dropPerDeath;
        public  IReadOnlyCollection<CoreDataPreset.Drop> DropPerDeath => _dropPerDeath;

        private List<CoreDataPreset.Drop>                _dropPerDamage;
        public  IReadOnlyCollection<CoreDataPreset.Drop> DropPerDamage => _dropPerDamage;
        
#endregion

        public CoreData()
        {
            _maxHealthPoints = 0;
            _healthPoints    = 0;

            _rebootDuration = 0.0f;
            _rebootTime     = 0.0f;
            
            _dealDamageDelay = 0.0f;
            _dealDamageTime  = 0.0f;
        }

        public void Reset()
        {
            _healthPoints   = _maxHealthPoints;
            _rebootTime     = 0.0f;
            _dealDamageTime = 0.0f;
        }
    }

    [Serializable, HideLabel, InlineProperty]
    public struct CoreDataPreset : IDataPreset
    {
        public int   HealthPoints;
        [Unit(Units.Second)]
        public float RebootDuration;
        [Unit(Units.Second)]
        public float DealDamageDelay;

        [Serializable]
        public struct Drop
        {
            public string ResourceIdentity;
            public int    ResourceQuantity;
        }
        
        public List<Drop> DropPerDamage;
        public List<Drop> DropPerDeath;
    }
}