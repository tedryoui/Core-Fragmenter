using _.Scripts.Gameplay.World_Modules;
using _.Scripts.Services;
using Unity.Mathematics;
using UnityEngine;
using VContainer.Unity;

namespace _.Scripts.Entry_Points
{
    public class GameplaySceneEntryPoint : IInitializable, IStartable
    {
        private ServiceLocator _serviceLocator;
        
        public GameplaySceneEntryPoint(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }
        
        public void Initialize()
        {
            _serviceLocator.Add(new WorldService(), "World Service");
            
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
        }

        public void Start()
        {
            var worldService = _serviceLocator.Get<WorldService>();

            var emitInformation = EntityEmittingModule.EmitInformation
                .Create(
                    EntityIdentityEnumFactory.BuildIdentity(EntityIdentityEnumFactory.EntityIdentityEnum.ENT_PLAYER)
                )
                .WithName("PLAYER")
                .WithPosition(float3.zero)
                .WithRotation(quaternion.identity)
                .WithScale(new float3(1.0f, 1.0f, 1.0f));

            worldService.EntityEmittingModule.Emit(emitInformation);
        }
    }
}