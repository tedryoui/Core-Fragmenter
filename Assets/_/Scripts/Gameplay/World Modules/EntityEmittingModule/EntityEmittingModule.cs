using System;
using System.Threading;
using System.Threading.Tasks;
using _.Scripts.Gameplay.Entity;
using _.Scripts.Scriptable_Objects;
using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.Services;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace _.Scripts.Gameplay.World_Modules
{
    public class EntityEmittingModule
    {
        public class EmitInformation
        {
            private string _identity;
            private string     _name;
            private float3     _position;
            private quaternion _rotation;
            private float3     _scale;

            private Action onComplete;

            public string Identity => _identity;
            public string Name => _name;
            public float3 Position => _position;
            public quaternion Rotation => _rotation;
            public float3 Scale => _scale;
            
            public Action OnComplete => onComplete;

            private EmitInformation() { }

            public static EmitInformation Create([NotNull] string _identity)
            {
                return new EmitInformation()
                {
                    _identity = _identity
                };
            }

            public EmitInformation WithName([NotNull] string name)
            {
                this._name = name;
                return this;
            }

            public EmitInformation WithPosition(float3 position)
            {
                this._position = position;
                return this;
            }

            public EmitInformation WithRotation(quaternion rotation)
            {
                this._rotation = rotation;
                return this;
            }

            public EmitInformation WithScale(float3 scale)
            {
                this._scale = scale;
                return this;
            }

            public EmitInformation SetOnComplete(Action onComplete)
            {
                this.onComplete = onComplete;
                return this;
            }
        }

        private WorldService            _worldService;
        private ScriptableObjectService _scriptableObjectService;
        
        private EntitiesCollectionScriptableObject _entitiesCollection;

        public EntityEmittingModule() { }
        
        [Inject]
        public void Configure(ServiceLocator serviceLocator)
        {
            _worldService = serviceLocator.Get<WorldService>();
            _scriptableObjectService = serviceLocator.Get<ScriptableObjectService>();
            
            _entitiesCollection = _scriptableObjectService.Find<EntitiesCollectionScriptableObject>();
        }

        public async UniTaskVoid Emit(EmitInformation emitInformation, CancellationToken cancellationToken = default)
        {
            var element    = _entitiesCollection.Get(emitInformation.Identity);
            var entity = (IEntity)null;

            if (element.UsePool)
                entity = await ObtainGameObject(element.EntityScriptableObject);
            else
                entity = await CreateEntity(element.EntityScriptableObject);
            
            if (element.EntityScriptableObject.Type is EntityScriptableObject.EntityType.Mono)
            {
                var monoEntity = (MonoEntity)entity;
                
                monoEntity.transform.position   = emitInformation.Position;
                monoEntity.transform.rotation   = emitInformation.Rotation;
                monoEntity.transform.localScale = emitInformation.Scale;
            }
        
            RegisterEntityInWorld(element, entity);
            
            emitInformation.OnComplete?.Invoke();
        }

        private async UniTask<IEntity> ObtainGameObject(EntityScriptableObject scriptableObject)
        {
            return await CreateEntity(scriptableObject);
        }

        private async UniTask<IEntity> CreateEntity(EntityScriptableObject scriptableObject)
        {
            IEntity entity;
            
            if (scriptableObject.Type is EntityScriptableObject.EntityType.Mono)
            {
                var result = await Object.InstantiateAsync(scriptableObject.MonoEntityPrefab);

                entity = result[0];
            }
            else
            {
                entity = (IEntity)Activator.CreateInstance(scriptableObject.AbstractEntityType);
            }
            
            return entity;
        }

        private void RegisterEntityInWorld(EntitiesCollectionScriptableObject.Element element, IEntity entity)
        {
            _worldService.Register(element.EntityScriptableObject.Identity, entity);
        }
    }
}