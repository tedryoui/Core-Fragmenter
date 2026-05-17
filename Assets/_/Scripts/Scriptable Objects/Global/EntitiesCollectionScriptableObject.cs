using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Global
{
    [CreateAssetMenu(fileName = "Entities Collection", menuName = "Core Fragmenter/Entities Collection", order = 0)]
    public class EntitiesCollectionScriptableObject : ScriptableObject
    {
        [Serializable]
        public struct Element
        {
#if UNITY_EDITOR

            private void ValidateEntityScriptableObjectAssignment()
            {
                if (EntityScriptableObject == null) return;
                
                if (EntityScriptableObject.Type is EntityScriptableObject.EntityType.Default)
                {
                    UsePool = false;
                    PoolSize = 0;
                }
            }

            [OnValueChanged("ValidateEntityScriptableObjectAssignment", InvokeOnInitialize = true, InvokeOnUndoRedo = true)]
#endif
            public EntityScriptableObject EntityScriptableObject;

            [PropertySpace] 
            public bool UseInjection;
            
            [BoxGroup("Pool Information")]
            public bool UsePool;
            [ShowIf("UsePool"), BoxGroup("Pool Information")]
            public int  PoolSize;
        }
        
        [SerializeField] private List<Element> _entities;

        public IReadOnlyCollection<Element> Entities => _entities.AsReadOnly();

        public Element Get(string identity = "")
        {
            var result = Entities.FirstOrDefault(x => x.EntityScriptableObject.Identity.Equals(identity));

            if (result.EntityScriptableObject == null)
                throw new KeyNotFoundException($"Entity {identity} not found!");

            return result;
        }
    }
}