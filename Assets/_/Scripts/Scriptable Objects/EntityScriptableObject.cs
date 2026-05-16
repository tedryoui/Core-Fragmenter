using System;
using _.Scripts.Gameplay.Entity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    public abstract class EntityScriptableObject : ScriptableObject
    {
        public enum EntityType { Default, Mono }
        
        [SerializeField] private string     _identity;
        
        [PropertySpace(10)]
        [SerializeField] private EntityType _type;
        [ShowIf("@this._type == EntityType.Mono")]
        [SerializeField] private MonoEntity _monoEntityPrefab;
        [ShowIf("@this._type == EntityType.Default")]
        [SerializeField] private string _abstractEntityType;

        public string Identity => _identity;

        public EntityType Type               => _type;
        public MonoEntity MonoEntityPrefab   => _monoEntityPrefab;
        public Type       AbstractEntityType => null;
    }
}