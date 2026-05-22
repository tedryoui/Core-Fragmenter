using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Gameplay.Entity.Concrete;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _.Scripts.Gameplay.Utility
{
    public class Player_Debugger : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private PlayerEntity _entity;

        [SerializeField]
        [ProgressBar(min: 0f, maxGetter: "GetPlayerSpeed")]
        private float _currentSpeed; 

        [SerializeField]
        private List<string> _availableBlueprints;
        
        [SerializeField]
        private List<string> _unlockedBlueprints;

        [SerializeField] 
        private List<string> _resources;

        private void Update()
        {
            if (_entity == null)
                _entity = GetComponent<PlayerEntity>();
            
            _currentSpeed = _entity.PlayerData.CurrentSpeed;

            _availableBlueprints = _entity.PlayerData.Unlock.AvailableBlueprints.ToList();
            _unlockedBlueprints = _entity.PlayerData.Unlock.UnlockedBlueprints.ToList();
            
            _resources = _entity.PlayerData.Resource.Resources.Select(x => $"{x.Key}: {x.Value}").ToList();
        }

        private float GetPlayerSpeed()
        {
            if (EditorApplication.isPlaying && _entity != null && _entity.PlayerData != null)
                return _entity.PlayerData.Speed;
            else
                return 0.0f;
        }
    }
}