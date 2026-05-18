using System;
using _.Scripts.Gameplay.Entity.Concrete;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace _.Scripts.Gameplay.Utility
{
    public class Drone_Debugger : MonoBehaviour
    {
        [SerializeField, DisableIf("@true")] private DroneEntity _entity;

        [HideInEditorMode] [ShowInInspector] private string _currentStateIdentity;

        [HideInEditorMode]
        [ProgressBar(min: 0.0f, maxGetter: "@this._entity.DroneData.DealDamageDelay")]
        [ShowInInspector] private float _dealDamageTime;

        [HideInEditorMode] 
        [ShowInInspector] private float3 _agentVelocity;

        private void OnValidate()
        {
            if (_entity == null)
                _entity = GetComponent<DroneEntity>();
        }

        private void Update()
        {
            _currentStateIdentity = _entity.CurrentState.Identity;
            
            _dealDamageTime = _entity.DroneData.DealDamageTime;
            
            _agentVelocity = _entity.NavMeshAgent.velocity;
        }
    }
}