using PrimeTween;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class PlayerMoveState : AbstractState<PlayerEntity>
    {
        public PlayerMoveState(PlayerEntity entity) : base(entity)
        {
        }

        public override string Identity => "Player Move";

        private float2 _lastTouchPosition;
        private float2 _direction;
        private quaternion _targetRotation;
        private float  _speed;

        private int AnimatorSpeedID => Animator.StringToHash("speed");

        private float TargetSpeed => 1.3f;
        private float AngularSpeed => 4.0f;
        
        public override void   OnEnter()
        {
            _lastTouchPosition = Entity.Input.General.TouchPosition.ReadValue<Vector2>();
            
            _speed = TargetSpeed;
            Entity.Animator.SetFloat(AnimatorSpeedID, _speed);
        }

        public override void   OnUpdate()
        {
            var touchPosition = (float2)Entity.Input.General.TouchPosition.ReadValue<Vector2>();
            var delta         = touchPosition - _lastTouchPosition;
            
            if (math.length(delta) != 0.0f)
            {
                _direction      = math.normalize(delta);
                _targetRotation = quaternion
                    .LookRotation(
                        new float3(_direction.x, 0.0f, _direction.y),
                        new float3(0.0f,         1.0f, 0.0f)
                    );
            }

            if (Entity.Animator.transform.rotation != _targetRotation)
            {
                var nextRotation = math.slerp(Entity.Animator.transform.rotation, _targetRotation, AngularSpeed * Time.deltaTime);
                Entity.Animator.transform.rotation = nextRotation;
            }
            
            if (Entity.Input.General.IsTouching.WasReleasedThisFrame())
            {
                Entity.SetState("Player Idle");
            }
        }

        public override void   OnExit()
        {
            _speed = 0.0f;
            Entity.Animator.SetFloat(AnimatorSpeedID, _speed);
        }
    }
}