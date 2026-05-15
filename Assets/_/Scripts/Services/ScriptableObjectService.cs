using System.Collections.Generic;
using UnityEngine;

namespace _.Scripts.Services
{
    public class ScriptableObjectService : IService
    {
        private Dictionary<string, ScriptableObject> _scriptableObjects = new Dictionary<string, ScriptableObject>();
        
        public IReadOnlyDictionary<string, ScriptableObject> Values => _scriptableObjects;
        
        public void Initialize()
        {
            _scriptableObjects = new Dictionary<string, ScriptableObject>();
            
            Debug.Log($"<color=green>{nameof(ScriptableObjectService)} initialized!</color>");
        }

        public void Dispose()
        {
            
        }

        public void Add(ScriptableObject scriptableObject, string identity)
        {
            if (_scriptableObjects.TryAdd(identity, scriptableObject))
            {
                Debug.Log($"<color=yellow>{identity} has been added!</color>");
            }
            else
            {
                Debug.Log($"<color=red>{identity} hasn't been added!</color>");
            }
        }

        public void Remove(string identity)
        {
            if (_scriptableObjects.Remove(identity))
            {
                Debug.Log($"<color=yellow>{identity} has been removed!</color>");
            }
            else
            {
                Debug.Log($"<color=red>{identity} hasn't been removed!</color>");
            }
        }
    }
}