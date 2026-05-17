using System;
using _.Scripts.Gameplay.Entity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    public abstract class EntityScriptableObject : ScriptableObject
    {
        public enum EntityType { Default, Mono }
        
        [BoxGroup("Entity Information")]
        [SerializeField] private string     _identity;
        
        [PropertySpace(10)]
        [BoxGroup("Entity Information")]
        [SerializeField] private EntityType _type;
        [ShowIf("@this._type == EntityType.Mono")]
        [BoxGroup("Entity Information")]
        [SerializeField] private MonoEntity _monoEntityPrefab;

#if UNITY_EDITOR
        private ValueDropdownList<string> FriendlyEntityTypeList()
        {
            var typeCollection = TypeCache.GetTypesDerivedFrom<AbstractEntity>();
            var list = new ValueDropdownList<string>();
            
            foreach (var type in typeCollection)
            {
                var typeName         = type.Name;
                var typeAssemblyName = type.AssemblyQualifiedName;
                
                list.Add(typeName, typeAssemblyName);
            }
            
            return list;
        }
        
        [ValueDropdown("FriendlyEntityTypeList", FlattenTreeView = true, IsUniqueList = true)]
#endif
        [ShowIf("@this._type == EntityType.Default")]
        [BoxGroup("Entity Information")]
        [SerializeField] private string _abstractEntityType;

        public string Identity => _identity;

        public EntityType Type               => _type;
        public MonoEntity MonoEntityPrefab   => _monoEntityPrefab;
        public Type       AbstractEntityType => System.Type.GetType(_abstractEntityType);
    }
}