using System.Linq;
using System.Threading;
using _.Scripts.Gameplay.Entity.Concrete;
using _.Scripts.Gameplay.Utility.Extensions;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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

            SetDroneDataValues();
            EnsureAgentHasProperValues();

            if (_cancellationTokenSource is {IsCancellationRequested: false})
                _cancellationTokenSource.Cancel();
            
            _cancellationTokenSource = new CancellationTokenSource();

            _target = Entity.WorldService.Entities[Entity.DroneData.TargetIdentity] as MonoEntity;

            TackNonChangedPositionSituation(cancellationToken: _cancellationTokenSource.Token);
            RefreshTargetPosition();
            WaitUntilTargetPositionChanges(cancellationToken: _cancellationTokenSource.Token);
        }

        private void EnsureAgentHasProperValues()
        {
            Entity.NavMeshAgent.speed        = Entity.DroneData.CurrentSpeed;
            Entity.NavMeshAgent.angularSpeed = Entity.DroneData.CurrentAngularSpeed;
        }

        private void SetDroneDataValues()
        {
            Entity.DroneData.CurrentAngularSpeed = Entity.DroneData.AngularSpeed;
            Entity.DroneData.CurrentSpeed        = Entity.DroneData.Speed;
        }

        private void ResetDroneDataValues()
        {
            Entity.DroneData.CurrentAngularSpeed = 0.0f;
            Entity.DroneData.CurrentSpeed        = 0.0f;
        }

        private async UniTaskVoid WaitUntilTargetPositionChanges(int checkDelay = 100,
            CancellationToken cancellationToken = default)
        {
            var compositeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                Application.exitCancellationToken,
                Entity.destroyCancellationToken,
                _target.destroyCancellationToken);
            var compositeCancellationToken = compositeCancellationTokenSource.Token;
            
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await UniTask
                    .Delay(
                        checkDelay, 
                        cancellationToken: compositeCancellationToken, 
                        cancelImmediately: true
                    )
                    .SuppressCancellationThrow();

                if (math.distance(_previousTargetPosition, _target.transform.position) >= 0.01f)
                    RefreshTargetPosition();
            }
        }

        private void RefreshTargetPosition()
        {
            _previousTargetPosition = _target.transform.position;
            Entity.NavMeshAgent.SetSoftDestination(_target.transform.position, validator: ValidatePosition);
        }

        private bool ValidatePosition(float3 position)
        {
            var entities = Entity.WorldService.Entities;
            var droneEntities = entities
                .Where(x => x.Value is DroneEntity)
                .Select(x => x.Value)
                .Cast<DroneEntity>();

            return droneEntities.Any(HasClosedEnoughPosition);
        }

        private bool HasClosedEnoughPosition(DroneEntity other)
        {
            var selfPosition = Entity.DroneData.Position;
            var otherPosition = other.DroneData.Position;

            return math.distance(selfPosition, otherPosition) <= Entity.NavMeshAgent.radius;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            
            if (Entity.NavMeshAgent.remainingDistance <= Entity.NavMeshAgent.stoppingDistance)
                Entity.SetState("Drone Shoot Target");
        }

        public override void OnExit()
        {
            base.OnExit();
            
            ResetDroneDataValues();
            EnsureAgentHasProperValues();
            
            _cancellationTokenSource.Cancel();
        }

        private async UniTaskVoid TackNonChangedPositionSituation(CancellationToken cancellationToken = default)
        {
            var compositeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                Application.exitCancellationToken,
                Entity.destroyCancellationToken);
            var compositeCancellationToken = compositeCancellationTokenSource.Token;

            var previousPosition = Entity.DroneData.Position;
            var timer            = 0.0f;
            
            while (!compositeCancellationToken.IsCancellationRequested)
            {
                var delta = math.distance(previousPosition, Entity.DroneData.Position);

                if (delta <= Entity.DroneData.NonChangePositionDelta)
                {
                    timer += Time.deltaTime;
                    if (timer >= Entity.DroneData.NonChangePositionDuration)
                    {
                        Entity.SetState("Drone Idle");
                        break;
                    }
                }
                else
                {
                    timer = 0.0f;
                }
                
                previousPosition = Entity.DroneData.Position;
                
                await UniTask.Yield();
            }
        }
    }
}