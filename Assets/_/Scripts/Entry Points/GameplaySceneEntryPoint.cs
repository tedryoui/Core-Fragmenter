using System.Collections.Generic;
using System.Linq;
using _.Scripts.Data.Concrete;
using _.Scripts.Gameplay.Entity.Concrete;
using _.Scripts.Gameplay.Player;
using _.Scripts.Gameplay.Utility.Extensions;
using _.Scripts.Gameplay.World_Modules;
using _.Scripts.Scriptable_Objects;
using _.Scripts.Scriptable_Objects.Concrete.Entities;
using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.Services;
using Sirenix.OdinInspector.Editor.Drawers;
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
            RegisterAmmoFabricAndItsData();
            RegisterDroneFabricAndItsData();
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

        private void RegisterAmmoFabricAndItsData()
        {
            var ammoFabric = Object.FindObjectsByType<TradeFabricEntity>()
                .FirstOrDefault(x => x.Identity.Equals("Ammo Fabric"));
            var soService = _serviceLocator.Get<ScriptableObjectService>();
            var tradesSO  = soService.Find<TradeConfigsCollectionScriptableObject>();
            var dataService =  _serviceLocator.Get<DataService>();
            var ammoFabricData = new TradeEntityData("Ammo Fabric");
            ammoFabricData.Fill(new TradeEntityDataPreset());
            
            ammoFabric.TradeList = new List<TradeConfigScriptableObject>()
            {
                tradesSO.Get("TRD_10_AMMO"),
            };
            
            dataService.Add(ammoFabricData);
        }

        private void RegisterDroneFabricAndItsData()
        {
            var droneFabric = Object.FindObjectsByType<DroneFabricEntity>()
                .FirstOrDefault(x => x.Identity.Equals("Drone Fabric"));
            var soService = _serviceLocator.Get<ScriptableObjectService>();
            var blueprintSO = soService.Find<BlueprintsCollectionScriptableObject>();
            var dataService = _serviceLocator.Get<DataService>();
            var droneFabricData = new DroneFabricData("Drone Fabric");
            droneFabricData.Fill(new DroneFabricDataPreset());

            droneFabric.Blueprints = new List<BlueprintScriptableObject>()
            {
                blueprintSO.Get("BPT_DRONE_1"),
                blueprintSO.Get("BPT_DRONE_2"),
                blueprintSO.Get("BPT_DRONE_3"),
            };
            
            dataService.Add(droneFabricData);
        }
    }
}