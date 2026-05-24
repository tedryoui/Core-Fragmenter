using System.Threading;
using _.Scripts.Gameplay.Entity.Concrete;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace @_.Scripts.Gameplay.Entity.State.Concrete.Drone
{
    public class DroneIdleState : AbstractState<DroneEntity>
    {
#region Enums

        public enum DroneTask { None, Deploy, SearchShootTarget }

#endregion

#region Abstract State overrides

        public override string Identity => "Drone Idle";

#endregion

#region Fields & Properties

        private DroneTask _taskPreset;
        private DroneTask _task;
        
        private CancellationTokenSource _cancellationTokenSource;

#endregion        
        
        public DroneIdleState(DroneEntity entity) : base(entity)
        {
            _task = DroneTask.None;
        }

        public void SetNextTask(DroneTask task)
        {
            _taskPreset = task;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (_cancellationTokenSource is { IsCancellationRequested: false })
                _cancellationTokenSource.Cancel();
            
            _task                    = DroneTask.None;
            _cancellationTokenSource = new CancellationTokenSource();
            
            if (_taskPreset is not DroneTask.None)
            {
                _task       = _taskPreset;
                _taskPreset = DroneTask.None;

                ExecuteTask();
            }
            else
            {
                _task = FindTask();
             
                if (_task is not DroneTask.None)
                    ExecuteTask();
                else 
                    WaitForTask(_cancellationTokenSource.Token);
            }
        }

        private void ExecuteTask()
        {
            switch (_task)
            {
                case DroneTask.Deploy:
                    Entity.SetState("Drone Deploy");
                    break;
                case DroneTask.SearchShootTarget:
                    Entity.SetState("Drone Search Shoot Target");
                    break;
            }
        }

        private async UniTaskVoid WaitForTask(CancellationToken cancellationToken = default)
        {
            var compositeTokenSource = CancellationTokenSource
                .CreateLinkedTokenSource(
                    cancellationToken,
                    Application.exitCancellationToken, 
                    Entity.destroyCancellationToken
                );
            var compositeTask = compositeTokenSource.Token;
            
            while (!compositeTask.IsCancellationRequested)
            {
                await UniTask
                    .Delay(
                        500, 
                        cancellationToken: compositeTask, 
                        cancelImmediately: true)
                    .SuppressCancellationThrow();

                if (compositeTask.IsCancellationRequested)
                    break;
                
                _task = FindTask();
                if (_task is not DroneTask.None)
                    ExecuteTask();
            }
        }

        private DroneTask FindTask()
        {
            if (Entity.DroneData.AmmoCurrentAmount != 0)
                return DroneTask.SearchShootTarget;
            else 
                return DroneTask.None;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _cancellationTokenSource.Cancel();
        }
    }
}