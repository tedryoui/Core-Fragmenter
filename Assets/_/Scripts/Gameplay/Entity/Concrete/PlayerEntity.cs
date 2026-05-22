using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Data.Concrete;
using _.Scripts.Events;
using _.Scripts.Gameplay.Player;
using _.Scripts.Gameplay.Utility;
using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.Services;
using Core.Scripts.Helpers;
using Unity.Mathematics;
using UnityEngine;
using VContainer;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class PlayerEntity : MonoEntity
    {
#region VContainer

        private PlayerProfile           _playerProfile;
        private DataService             _dataService;
        private ScriptableObjectService _scriptableObjectService;

        [Inject]
        private void Configure(ServiceLocator serviceLocator, IObjectResolver resolver)
        {
            _dataService             = serviceLocator.Get<DataService>();
            _scriptableObjectService = serviceLocator.Get<ScriptableObjectService>();
            _playerProfile           = resolver.Resolve<PlayerProfile>();
        }

#endregion

#region Mono Entity override

        public override string Identity => $"Player GUID: {gameObject.GetEntityId()}";

        public override List<AbstractState> PossibleStates => new List<AbstractState>()
        {
            new PlayerIdleState(this),
            new PlayerMoveState(this)
        };

#endregion

#region Scene Reference

        [SerializeField] private Animator         _animator;
        [SerializeField] private OnAnimatorEvents _onAnimatorEvents;
        [SerializeField] private Rigidbody        _rigidbody;
        
        public Animator         Animator         => _animator;
        public OnAnimatorEvents OnAnimatorEvents => _onAnimatorEvents;
        public Rigidbody        Rigidbody        => _rigidbody;

#endregion

#region Fields

        private PlayerData _playerData;
        public PlayerData PlayerData => _playerData ??= _dataService.Get<PlayerData>(_playerProfile.ID);

        private InputSystem_Actions _input;
        public  InputSystem_Actions Input => _input;

        private float3     _animationMoveDelta     = float3.zero;
        private quaternion _animationRotationDelta = quaternion.identity;

        [SerializeField] private float _stoppingDistance;
        [SerializeField] private float _stoppingOffset;

        public float StoppingDistance => _stoppingDistance;
        public float StoppingOffset => _stoppingOffset;

#endregion

        private void Awake()
        {
            _input = new InputSystem_Actions();
        }

        public override void Start()
        {
            base.Start();
            
            OnAnimatorEvents.onAnimatorMoved += OnAnimatorMoved;
            
            EventBus.Instance.Subscribe<AddBlueprintEvent>(OnAddBlueprintEvent);
            EventBus.Instance.Subscribe<UnlockBlueprintEvent>(OnUnlockBlueprintEvent);
            EventBus.Instance.Subscribe<RemoveBlueprintEvent>(OnRemoveBlueprintEvent);
        }

        private bool DoesBlueprintExist(string identity)
        {
            var collection = _scriptableObjectService.Find<BlueprintsCollectionScriptableObject>();
            var hasSO      = collection.Blueprints.Any(x => x.Identity.Equals(identity));

            return hasSO;
        }

        private void OnRemoveBlueprintEvent(RemoveBlueprintEvent obj)
        {
            if (DoesBlueprintExist(obj.Identity))
                PlayerData.Unlock.RemoveBlueprint(obj.Identity);
        }

        private void OnAddBlueprintEvent(AddBlueprintEvent e)
        {
            if (DoesBlueprintExist(e.Identity))
                PlayerData.Unlock.AddBlueprint(e.Identity, e.Unlocked);
        }

        private void OnUnlockBlueprintEvent(UnlockBlueprintEvent e)
        {
            if (DoesBlueprintExist(e.Identity))
                PlayerData.Unlock.UnlockBlueprint(e.Identity);
        }

        private void OnAnimatorMoved(Animator animator)
        {
            _animationMoveDelta     += (float3)animator.deltaPosition;
            _animationRotationDelta =  _animator.deltaRotation * _animationRotationDelta; 
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void FixedUpdate()
        {
            SyncRigidbodyWithData();
        }

        private void SyncRigidbodyWithData()
        {
            if (math.lengthsq(_animationMoveDelta) >= 0.0001f || !_animationRotationDelta.Equals(quaternion.identity))
            {
                Rigidbody.MovePosition(Rigidbody.position + (Vector3)_animationMoveDelta);
                Rigidbody.MoveRotation(PlayerData.Rotation * (Quaternion)_animationRotationDelta);
            
                _animationMoveDelta     = float3.zero;
                _animationRotationDelta = quaternion.identity;
            }
            
            PlayerData.Position = _rigidbody.position;
            PlayerData.Rotation = _rigidbody.rotation;
        }

        private void OnDestroy()
        {
            _input.Disable();
            _input.Dispose();
            
            EventBus.Instance.Unsubscribe<AddBlueprintEvent>(OnAddBlueprintEvent);
            EventBus.Instance.Unsubscribe<UnlockBlueprintEvent>(OnUnlockBlueprintEvent);
            EventBus.Instance.Unsubscribe<RemoveBlueprintEvent>(OnRemoveBlueprintEvent);
        }

        private void OnDrawGizmosSelected()
        {
            var from = transform.position + Vector3.up + transform.forward * _stoppingOffset;
            var to   = from + transform.forward * _stoppingDistance;
            
            Gizmos.color =Color.black;
            Gizmos.DrawLine(from, to);
        }
    }
}