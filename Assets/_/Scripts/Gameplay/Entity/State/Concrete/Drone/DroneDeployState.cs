using System.Threading;
using _.Scripts.Gameplay.Entity.Concrete;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneDeployState : AbstractState<DroneEntity>
    {
#region Abstract State overrides

        public override string Identity => "Drone Deploy";

#endregion

#region Fields & Properties

        private CancellationTokenSource _cancellationTokenSource;

#endregion

        public DroneDeployState(DroneEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (_cancellationTokenSource is { IsCancellationRequested: false })
                _cancellationTokenSource.Cancel();
            
            _cancellationTokenSource = new CancellationTokenSource();

            WaitAndCompleteDeploy(_cancellationTokenSource.Token);
        }

        private async UniTaskVoid WaitAndCompleteDeploy(CancellationToken cancellationToken = default)
        {
            var compositeCancellationTokenSource = CancellationTokenSource
                .CreateLinkedTokenSource(
                    cancellationToken,
                    Application.exitCancellationToken,
                    Entity.destroyCancellationToken
                );
            var compositeCancellationToken = compositeCancellationTokenSource.Token;

            await UniTask
                .Delay(
                    500,
                    cancellationToken: compositeCancellationToken,
                    cancelImmediately: true)
                .SuppressCancellationThrow();
            
            Entity.SetState("Drone Idle");
        }
    }
}