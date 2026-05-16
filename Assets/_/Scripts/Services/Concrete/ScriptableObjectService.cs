using System.Collections.Generic;
using System.Linq;
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

        public T Find<T>(string identity = "") 
        where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(identity))
            {
                var result = Values.FirstOrDefault(x => x.Value.GetType() == typeof(T));
                
                if (result.Value != null) 
                    return result.Value as T;
                else 
                    throw new KeyNotFoundException($"<color=white>Scriptable Object with type {nameof(T)} was not found!</color>");
            }
            else
            {
                if (Values.TryGetValue(identity, out var scriptableObject)) 
                    return scriptableObject as T;
                else 
                    throw new KeyNotFoundException($"<color=white>Scriptable Object with identity {identity} was not found!</color>");
            }
        }
    }
}