using System.Collections.Generic;
using System.Threading;
using _.Scripts.Data.Concrete;
using _.Scripts.Gameplay.Entity.State.Concrete.Drone;
using _.Scripts.Gameplay.Player;
using _.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class DroneEntity : MonoEntity
    {
#region VContainer

        [Inject] private ServiceLocator _serviceLocator;
        [Inject] private PlayerProfile  _playerProfile;

#endregion

#region Mono Entity overrides

        public override string Identity => _identity;

#endregion

#region Fields & Properties

        private string _identity;

        public void AssignIdentity(string identity)
        {
            _identity = identity;
        }

        [SerializeField] private NavMeshAgent _navMeshAgent;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        
        private WorldService _worldService;
        public WorldService WorldService => _worldService ??= _serviceLocator.Get<WorldService>();
        
        private DataService _dataService;
        public DataService DataService => _dataService ??= _serviceLocator.Get<DataService>();
        
        private DroneData _droneData;
        public DroneData DroneData => _droneData ??= DataService.Get<DroneData>(Identity);

#endregion

        public override List<AbstractState> PossibleStates => new()
        {
            new DroneIdleState(this),
            new DroneDeployState(this),
            new DroneSearchShootTargetState(this),
            new DroneFollowShootTargetState(this),
            new DroneShootTargetState(this)
        };

        public override void Start()
        {
            base.Start();

            NavMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 100);
        }

        public override void Update()
        {
            base.Update();

            SyncEntityWithData();
        }

        private void SyncEntityWithData()
        {
            if (NavMeshAgent.updatePosition)
                DroneData.Position = this.transform.position;
            else 
                this.transform.position = DroneData.Position;
            
            if (NavMeshAgent.updateRotation)
                DroneData.Rotation = this.transform.rotation;
            else
                this.transform.rotation = DroneData.Rotation;
        }
    }
}