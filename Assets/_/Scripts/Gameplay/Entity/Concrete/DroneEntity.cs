using System.Collections.Generic;
using System.Threading;
using _.Scripts.Gameplay.Entity.State.Concrete.Drone;
using Cysharp.Threading.Tasks;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class DroneEntity : MonoEntity
    {
        public override string Identity => "Drone";

        public override List<AbstractState> PossibleStates => new()
        {
            new DroneIdleState(this),
            new DroneReachTargetState(this),
            new DroneShootTargetState(this),
        };

        public override void Start()
        {
            base.Start();

            SwitchStates(gameObject.GetCancellationTokenOnDestroy());
        }

        private async UniTaskVoid SwitchStates(CancellationToken token = default)
        {
            while (true)
            {
                if (token.IsCancellationRequested) return;
                 
                SetState("Drone Idle");
                
                await UniTask.Delay(2500, cancellationToken: token);
                
                SetState("Drone Reach Target");
                
                await UniTask.Delay(2500, cancellationToken: token);
                
                SetState("Drone Shoot Target");
                
                await UniTask.Delay(2500, cancellationToken: token);
            }
        }
    }
}