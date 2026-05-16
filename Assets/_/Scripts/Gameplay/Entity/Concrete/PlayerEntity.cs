using System;
using System.Collections.Generic;
using _.Scripts.Gameplay.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class PlayerEntity : MonoEntity
    {
        public override string Identity => $"Player GUID: {gameObject.GetEntityId()}";

        public override List<AbstractState> PossibleStates => new List<AbstractState>()
        {
            new PlayerIdleState(this),
            new PlayerMoveState(this)
        };
        
        [SerializeField] private Animator       _animator;
        [SerializeField] private OnAnimatorEvents _onAnimatorEvents;
        [SerializeField] private Rigidbody _rigidbody;
        
        public Animator         Animator       => _animator;
        public OnAnimatorEvents OnAnimatorEvents => _onAnimatorEvents;
        public Rigidbody Rigidbody => _rigidbody;

        private InputSystem_Actions _input;
        public  InputSystem_Actions Input => _input;

        private void Awake()
        {
            _input = new InputSystem_Actions();
        }

        public override void Start()
        {
            base.Start();
            
            OnAnimatorEvents.onAnimatorMoved += OnAnimatorMoved;
        }

        private void OnAnimatorMoved(Animator animator)
        {
            Rigidbody.MovePosition(Rigidbody.position + animator.deltaPosition);
            Rigidbody.MoveRotation(Rigidbody.rotation * animator.deltaRotation);
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void Update()
        {
            CurrentState?.OnUpdate();
        }

        private void OnDestroy()
        {
            _input.Disable();
            _input.Dispose();
        }
    }
}