using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Scriptable_Objects;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace _.Scripts.Data.Concrete
{
    public class PlayerData : IData<PlayerDataPreset>
    {
        private string _identity;

        public string Identity => _identity;

        public bool   IsSigned => true;

#region Structures

        [Serializable]
        public class UnlockListing
        {
            private HashSet<string> _availableBlueprints;
            private HashSet<string> _unlockedBlueprints;
            
            public UnlockListing()
            {
                _availableBlueprints = new HashSet<string>();
                _unlockedBlueprints = new HashSet<string>();
            }
            
            public void AddBlueprint(string identity, bool isUnlocked = false)
            {
                if (!_availableBlueprints.Add(identity))
                    Debug.Log($"{identity} blueprint is already in use!");
                else if (isUnlocked)
                    _unlockedBlueprints.Add(identity);
            }
            
            public void AddBlueprint(IEnumerable<string> identities, bool isUnlocked = false)
            {
                foreach (var identity in identities)    
                    AddBlueprint(identity, isUnlocked);
            }

            public void AddBlueprint(IEnumerable<string> identities, IEnumerable<bool> isUnlocked = null)
            {
                if (isUnlocked != null && identities.Count() != isUnlocked.Count())
                    throw new ArgumentException($"{string.Join(", ", identities)} is {string.Join(", ", isUnlocked)}!");
                
                for (int index = 0; index < identities.Count(); index++)
                {
                    var identity = identities.ElementAt(index);
                    var lockState = (isUnlocked == null) ? false : isUnlocked.ElementAt(index);
                    
                    AddBlueprint(identity, lockState);
                }
            }

            public void UnlockBlueprint(string identity)
            {
                if (!_availableBlueprints.Contains(identity))
                    throw new KeyNotFoundException($"{identity} blueprint is not in use!");
                
                _unlockedBlueprints.Add(identity);
            }

            public void UnlockBlueprint(IEnumerable<string> identity)
            {
                foreach (var id in identity)
                {
                    if (!_availableBlueprints.Contains(id))
                        Debug.LogError($"{id} blueprint is not in use!");
                    
                    _unlockedBlueprints.Add(id);
                }
            }

            public void RemoveBlueprint(string identity)
            {
                if (!_availableBlueprints.Remove(identity))
                    Debug.Log($"{identity} blueprint is not in use!");
            }
            
            public bool HasBlueprint(string identity)
            {
                return _availableBlueprints.Contains(identity);
            }
            
            public bool IsUnlocked(string identity)
            {
                if (_availableBlueprints.Contains(identity))
                    return _unlockedBlueprints.Contains(identity);
                
                return false;
            }
        }

#endregion

#region Secondary Fields

        private float3     _position;
        private quaternion _rotation;
        private float      _currentSpeed;

#endregion

#region Primary Fields

        private float _speed;
        private float _angularSpeed;

        private UnlockListing _unlock;
        
#endregion

#region Secondary Properties

        public float3     Position
        {
            get => _position;
            set => _position = value;
        }

        public quaternion Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public float CurrentSpeed
        {
            get => _currentSpeed;
            set => _currentSpeed = value;
        }

#endregion

#region Primary Properties

        public float Speed        => _speed;
        public float AngularSpeed => _angularSpeed;

        public UnlockListing Unlock => _unlock;

#endregion

        public PlayerData(string identity)
        {
            _identity = identity;
            
            _position = float3.zero;
            _rotation = quaternion.identity;
            
            _currentSpeed = 0f;

            _unlock = new UnlockListing();
        }

        public IData<PlayerDataPreset> Fill(PlayerDataPreset presetData)
        {
            _speed = presetData.Speed;
            _angularSpeed = presetData.AngularSpeed;
            
            _unlock.AddBlueprint(presetData.DefaultBlueprints.Select(x => x.Identity), presetData.DefaultBlueprints.Select(x => x.IsUnlocked));

            return this;
        }
    }

    [Serializable, HideLabel, InlineProperty]
    public struct PlayerDataPreset : IDataPreset
    {
        [Serializable]
        public struct DefaultBlueprintPair
        {
            [HorizontalGroup]
            public string Identity;
            [HorizontalGroup(Width = 56)] [HideLabel] [SuffixLabel("unlock")]
            public bool IsUnlocked;
        }
        
        public float Speed;
        public float AngularSpeed;
        
        public List<DefaultBlueprintPair> DefaultBlueprints;
    }
}