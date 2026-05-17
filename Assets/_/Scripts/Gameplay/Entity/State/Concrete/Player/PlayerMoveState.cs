using _.Scripts.Gameplay.Utility.Extensions;
using Cysharp.Threading.Tasks.Triggers;
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

        private int AnimatorSpeedID => Animator.StringToHash("speed");
        
        public override void   OnEnter()
        {
            _lastTouchPosition = Entity.Input.General.TouchPosition.ReadValue<Vector2>();
            
            Entity.PlayerData.CurrentSpeed = Entity.PlayerData.Speed;
            Entity.Animator.SetFloat(AnimatorSpeedID, Entity.PlayerData.CurrentSpeed);
        }

        public override void   OnUpdate()
        {
            var touchPosition = (float2)Entity.Input.General.TouchPosition.ReadValue<Vector2>();
            var delta         = touchPosition - _lastTouchPosition;

            TryTransformPlayer(delta, out quaternion targetRotation);
            if (CheckForObstacles(delta))
            {
                if (Entity.PlayerData.CurrentSpeed == 0.0f)
                {
                    Entity.PlayerData.Rotation = targetRotation;
                    Entity.PlayerData.CurrentSpeed = Entity.PlayerData.Speed;
                    Entity.Animator.SetFloat(AnimatorSpeedID, Entity.PlayerData.CurrentSpeed);
                }
                else
                {
                    if (!Entity.PlayerData.Rotation.Equals(targetRotation))
                    {
                        Entity.PlayerData.Rotation = math.slerp(
                            Entity.PlayerData.Rotation, 
                            targetRotation, 
                            Entity.PlayerData.AngularSpeed * Time.deltaTime);
                    }
                }
            }
            else
            {
                if (Entity.PlayerData.CurrentSpeed != 0.0f)
                {
                    Entity.PlayerData.CurrentSpeed = 0.0f;
                    Entity.Animator.SetFloat(AnimatorSpeedID, Entity.PlayerData.CurrentSpeed);
                }
            }
            
            if (Entity.Input.General.IsTouching.WasReleasedThisFrame())
            {
                Entity.SetState("Player Idle");
            }
        }

        private bool CheckForObstacles(float2 delta)
        {
            var basePosition = Entity.transform.position + Vector3.up * 0.5f;
            var from         = basePosition + Entity.transform.forward * Entity.StoppingOffset;
            var ray          = new Ray(from, new Vector3(delta.x, 0.0f, delta.y));

            if (Physics.Raycast(ray, out var hit, Entity.StoppingDistance))
                return false;
            return true;
        }

        private void TryTransformPlayer(float2 delta, out quaternion targetRotation)
        {
            targetRotation = Entity.PlayerData.Rotation;
            
            if (math.length(delta) != 0.0f)
            {
                _direction      = math.normalize(delta);
                targetRotation = quaternion
                    .LookRotation(
                        new float3(_direction.x, 0.0f, _direction.y),
                        new float3(0.0f,         1.0f, 0.0f)
                    );
            }
        }

        public override void   OnExit()
        {
            Entity.PlayerData.CurrentSpeed = 0.0f;
            Entity.Animator.SetFloat(AnimatorSpeedID, Entity.PlayerData.CurrentSpeed);
        }
    }
}