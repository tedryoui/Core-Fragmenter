using System.Threading;
using _.Scripts.Gameplay.Entity.Concrete;
using _.Scripts.Gameplay.Utility.Extensions;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine.PlayerLoop;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneFollowShootTargetState : AbstractState<DroneEntity>
    {
#region Abstract State overrides

        public override string Identity => "Drone Follow Shoot Target";

#endregion

#region Fields & Properties
        
        private CancellationTokenSource _cancellationTokenSource;

        private MonoEntity _target;
        
        private float3 _previousTargetPosition;
        
#endregion

        public DroneFollowShootTargetState(DroneEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (_cancellationTokenSource is {IsCancellationRequested: false})
                _cancellationTokenSource.Cancel();
            
            _cancellationTokenSource = new CancellationTokenSource();

            _target = Entity.WorldService.Entities[Entity.DroneData.TargetIdentity] as MonoEntity;
            
            RefreshTargetPosition();
            WaitUntilTargetPositionChanges();
        }

        private async UniTaskVoid WaitUntilTargetPositionChanges(int checkDelay = 100,
            CancellationToken cancellationToken = default)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await UniTask
                    .Delay(
                        checkDelay, 
                        cancellationToken: cancellationToken, 
                        cancelImmediately: true
                    )
                    .SuppressCancellationThrow();

                if (math.distance(_previousTargetPosition, _target.transform.position) >= 0.01f)
                    RefreshTargetPosition();
            }
        }

        private void RefreshTargetPosition()
        {
            var hasClosestPoint = Entity.NavMeshAgent
                .GetNearestAccessiblePoint(
                    _target.transform.position,
                    10f,
                    out var closestPoint
                );

            if (hasClosestPoint)
            {
                _previousTargetPosition = closestPoint;
                Entity.NavMeshAgent.SetDestination(closestPoint);
            }
            else
            {
                Entity.SetState("Drone Idle");
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (Entity.NavMeshAgent.remainingDistance <= Entity.NavMeshAgent.stoppingDistance)
                Entity.SetState("Drone Idle");
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _cancellationTokenSource.Cancel();
        }
    }
}