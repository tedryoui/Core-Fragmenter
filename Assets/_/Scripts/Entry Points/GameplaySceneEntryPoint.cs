using _.Scripts.Data.Concrete;
using _.Scripts.Gameplay.Entity.Concrete;
using _.Scripts.Gameplay.Player;
using _.Scripts.Gameplay.Utility.Extensions;
using _.Scripts.Gameplay.World_Modules;
using _.Scripts.Scriptable_Objects.Concrete.Entities;
using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.Services;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _.Scripts.Entry_Points
{
    public class GameplaySceneEntryPoint : IInitializable, IStartable
    {
        private IObjectResolver _objectResolver;
        private ServiceLocator _serviceLocator;
        private PlayerProfile  _profile;
        
        public GameplaySceneEntryPoint(ServiceLocator serviceLocator, PlayerProfile profile, IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            _serviceLocator = serviceLocator;
            _profile        = profile;
        }
        
        public void Initialize()
        {
            _serviceLocator.Add(new WorldService(), "World Service");
            
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
        }

        public void Start()
        {
            CreatePlayerObjectAndItsData();        
            RegisterCoreAndItsData();

            CreateAndRegisterDrone();
        }

        private void CreatePlayerObjectAndItsData()
        {
            var dataService  = _serviceLocator.Get<DataService>();
            var worldService = _serviceLocator.Get<WorldService>();
            var playerData   = new PlayerData(_profile.ID);
            
            dataService.Add(playerData);
            
            var emitInformation = EntityEmittingModule.EmitInformation
                .Create(
                    EntityIdentityEnumFactory.BuildIdentity(EntityIdentityEnumFactory.EntityIdentityEnum.ENT_PLAYER)
                )
                .WithName("PLAYER")
                .WithPosition(playerData.Position)
                .WithRotation(playerData.Rotation)
                .WithScale(new float3(1.0f, 1.0f, 1.0f))
                .SetOnComplete((so) =>
                {
                    if (so is PlayerEntityScriptableObject playerEntityScriptableObject)
                        playerData.Fill(playerEntityScriptableObject.DataPreset);
                });
            worldService.EntityEmittingModule.Emit(emitInformation);
        }

        private void RegisterCoreAndItsData()
        {
            var scriptableObjectsService   = _serviceLocator.Get<ScriptableObjectService>();
            var worldService               = _serviceLocator.Get<WorldService>();
            var dataService                = _serviceLocator.Get<DataService>();
            var coreEntity                 = Object.FindAnyObjectByType<CoreEntity>(FindObjectsInactive.Include);
            var entitiesCollection         = scriptableObjectsService.Find<EntitiesCollectionScriptableObject>();
            var coreEntityScriptableObject = entitiesCollection.Get("Core").EntityScriptableObject as CoreEntityScriptableObject;
            var coreData                   = new CoreData();
            
            worldService.Register(coreEntity.Identity, coreEntity);
            
            coreData.Fill(coreEntityScriptableObject.DataPreset);
            coreData.Reset();
            dataService.Add(coreData);
            
            _objectResolver.Inject(coreEntity);
        }

        private void CreateAndRegisterDrone()
        {
            var dronePivot      = GameObject.Find("TEMP_DRONE_PIVOT_1");
            var otherDronePivot = GameObject.Find("TEMP_DRONE_PIVOT_2");
            var dataService     = _serviceLocator.Get<DataService>();
            var worldService    = _serviceLocator.Get<WorldService>();
            var droneData       = new DroneData("Drone_01");
            var otherDroneData  = new DroneData("Drone_02");

            var emitInformation = EntityEmittingModule.EmitInformation
                .Create("Drone")
                .WithPosition(dronePivot.transform.position)
                .WithRotation(dronePivot.transform.rotation)
                .WithScale(new float3(1.0f, 1.0f, 1.0f))
                .SetRegisterInWorldService(false)
                .SetOnComplete((so) =>
                {
                    if (so is DroneEntityScriptableObject droneEntityScriptableObject)
                    {
                        droneData.Fill(droneEntityScriptableObject.DataPreset);
                        otherDroneData.Fill(droneEntityScriptableObject.DataPreset);
                    }
                });
            
            dataService.Add(droneData);
            var operation = worldService.EntityEmittingModule.Emit(emitInformation).GetAwaiter();
            operation.OnCompleted(() =>
            {
                var result = operation.GetResult();
                
                if (result is DroneEntity droneEntity)
                {
                    droneEntity.AssignIdentity("Drone_01");
                    worldService.Register(droneEntity.Identity, droneEntity);
                }
            });
            
            dataService.Add(otherDroneData);
            operation = worldService.EntityEmittingModule.Emit(emitInformation).GetAwaiter();
            operation.OnCompleted(() =>
            {
                var result = operation.GetResult();
                
                if (result is DroneEntity droneEntity)
                {
                    droneEntity.AssignIdentity("Drone_02");
                    worldService.Register(droneEntity.Identity, droneEntity);
                }
            });
        }
    }
}