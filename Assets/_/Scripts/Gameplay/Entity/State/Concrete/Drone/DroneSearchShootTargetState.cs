using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _.Scripts.Gameplay.Entity.Concrete;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneSearchShootTargetState : AbstractState<DroneEntity>
    {
#region Abstract State overrides

        public override string Identity => "Drone Search Shoot Target";

#endregion

#region Fields & Properties

        private CancellationTokenSource cancellationTokenSource;

        private static List<Type> ShootableTypes => new List<Type>()
        {
            typeof(CoreEntity),
        };

#endregion

        public DroneSearchShootTargetState(DroneEntity entity) : base(entity)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (cancellationTokenSource is { IsCancellationRequested: false })
                cancellationTokenSource.Cancel();
            
            cancellationTokenSource = new CancellationTokenSource();

            WaitAndSearchForTarget(cancellationTokenSource.Token);
        }

        private async UniTaskVoid WaitAndSearchForTarget(CancellationToken cancellationToken = default)
        {
            var compositeCancellationTokenSource = CancellationTokenSource
                .CreateLinkedTokenSource(
                    cancellationToken,
                    Application.exitCancellationToken,
                    Entity.destroyCancellationToken
                );
            var compositeCancellationToken = compositeCancellationTokenSource.Token;

            Entity.DroneData.TargetIdentity = "";
            
            while (!compositeCancellationToken.IsCancellationRequested)
            {
                var worldService = Entity.WorldService;

                var sortedEnemies = worldService.Entities
                    .Select(x => x.Value)
                    .OfType<MonoEntity>()
                    .OrderBy(x => math.distance(x.transform.position, Entity.DroneData.Position))
                    .Where(x => ShootableTypes.Contains(x.GetType()))
                    .ToList();
                
                var targetIdentity = sortedEnemies.First().Identity;
                Entity.DroneData.TargetIdentity = targetIdentity;

                if (!string.IsNullOrEmpty(Entity.DroneData.TargetIdentity))
                    break;
                
                await UniTask
                    .Delay(
                        TimeSpan.FromSeconds(5),
                        cancellationToken: compositeCancellationToken,
                        cancelImmediately: true
                    )
                    .SuppressCancellationThrow();
            }
            
            if (cancellationToken.IsCancellationRequested)
                return;
            
            Entity.SetState("Drone Follow Shoot Target");
        }

        public override void OnExit()
        {
            base.OnExit();
            
            cancellationTokenSource.Cancel();
        }
    }
}