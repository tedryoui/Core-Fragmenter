using System;
using System.Threading;
using _.Scripts.Gameplay.Entity.Concrete;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneRefillAmmoState : AbstractState<DroneEntity>
    {
        public override string Identity => "Drone Refill Ammo";

        public DroneRefillAmmoState(DroneEntity entity) : base(entity)
        {
        }

        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public override void OnUpdate()
        {
            base.OnUpdate();

            var distance = math.distance(Entity.DroneData.Position, Entity.PlayerData.Position);

            if (distance <= 2f)
            {
                if (_cancellationTokenSource == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();

                    WaitForRefill(_cancellationTokenSource.Token).Forget();
                }
            }
            else
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
            }
        }

        private async UniTaskVoid WaitForRefill(CancellationToken cancellationToken)
        {
            while (!Entity.PlayerData.Resource.HasResource("RES_AMMO") ||
                   Entity.PlayerData.Resource.GetResourceQuantity("RES_AMMO") == 0)
                await UniTask.Yield();
            
            var hash  = HashCode.Combine(Entity.PlayerData.Position, Entity.DroneData.Rotation);
            var start = DateTime.Now;
            var end   = start + TimeSpan.FromSeconds(1.5f);

            while (DateTime.Compare(DateTime.Now, end) < 0)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                var currentPlayerHash = HashCode.Combine(Entity.PlayerData.Position, Entity.DroneData.Rotation);

                if (hash != currentPlayerHash)
                {
                    _cancellationTokenSource?.Cancel();
                    _cancellationTokenSource = null;
                    return;
                }
                
                await UniTask.Yield();
            }

            if (cancellationToken.IsCancellationRequested)
                return;

            Refill();
        }

        private void Refill()
        {
            if (!Entity.PlayerData.Resource.HasResource("RES_AMMO"))
                return;
            
            var playerAmmoAmount = Entity.PlayerData.Resource.GetResourceQuantity("RES_AMMO");
            var toRefill         = math.min(Entity.DroneData.AmmoMaximumAmount, playerAmmoAmount);
            
            if (toRefill == 0) 
                return;
            
            Entity.DroneData.AmmoCurrentAmount =+ toRefill;
            Entity.PlayerData.Resource.SetResourceQuantity("RES_AMMO", playerAmmoAmount - toRefill);
            Entity.SetState("Drone Idle");
        }
    }
}