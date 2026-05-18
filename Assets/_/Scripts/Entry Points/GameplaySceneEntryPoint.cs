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
using VContainer.Unity;

namespace _.Scripts.Entry_Points
{
    public class GameplaySceneEntryPoint : IInitializable, IStartable
    {
        private ServiceLocator _serviceLocator;
        private PlayerProfile  _profile;
        
        public GameplaySceneEntryPoint(ServiceLocator serviceLocator, PlayerProfile profile)
        {
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
        }
    }
}