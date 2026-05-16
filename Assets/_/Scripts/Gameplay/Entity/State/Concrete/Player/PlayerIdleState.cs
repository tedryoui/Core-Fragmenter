using UnityEngine;
using UnityEngine.InputSystem;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class PlayerIdleState : AbstractState<PlayerEntity>
    {
        public PlayerIdleState(PlayerEntity entity) : base(entity)
        {
        }

        public override string Identity => "Player Idle";
        
        public override void   OnEnter()
        {
            base.OnEnter();
        }

        public override void   OnUpdate()
        {
            if (Entity.Input.General.IsTouching.IsPressed() &&
                Entity.Input.General.TouchDelta.ReadValue<Vector2>().sqrMagnitude >= 10f)
            {
                Entity.SetState("Player Move");
            }
        }

        public override void   OnExit()
        {
            base.OnExit();
        }
    }
}