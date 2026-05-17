using System;
using System.Collections.Generic;
using System.Threading;
using _.Scripts.Gameplay.Entity;
using _.Scripts.Scriptable_Objects;
using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.Services;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using Object = System.Object;

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

        private Transform _objectPoolRoot;
        private Dictionary<string, ObjectPool<IEntity>> _objectPool;

        public EntityEmittingModule()
        {
            _objectPool = new Dictionary<string, ObjectPool<IEntity>>();
        }
        
        [Inject]
        public void Configure(ServiceLocator serviceLocator)
        {
            _worldService = serviceLocator.Get<WorldService>();
            _scriptableObjectService = serviceLocator.Get<ScriptableObjectService>();
            
            _entitiesCollection = _scriptableObjectService.Find<EntitiesCollectionScriptableObject>();

            CreateObjectPools();
        }

        private void CreateObjectPools()
        {
            CreateObjectPoolRootTransform();
            foreach (var entity in _entitiesCollection.Entities)
            {
                if (entity.UsePool)
                {
                    var root = CreateEntityPoolRootTransform(entity.EntityScriptableObject);
                    
                    var pool = new ObjectPool<IEntity>(
                        () => CreatePoolEntity(entity.EntityScriptableObject, root),
                        OnGetPoolEntity,
                        OnReleasePoolEntity,
                        OnDestroyPoolEntity,
                        defaultCapacity: entity.PoolSize,
                        maxSize: 999
                    );

                    if (_objectPool.TryAdd(entity.EntityScriptableObject.Identity, pool))
                        Debug.Log($"Registered pool for entity {entity.EntityScriptableObject.Identity}.");
                    else 
                        throw new Exception($"Failed to register pool for entity {entity.EntityScriptableObject.Identity}.");
                }
            }
        }

        private void CreateObjectPoolRootTransform()
        {
            _objectPoolRoot                    = new GameObject("ObjectPool").transform;
            _objectPoolRoot.transform.position = new Vector3(-999f, -999, -999);
        }

        private Transform CreateEntityPoolRootTransform(EntityScriptableObject scriptableObject)
        {
            var poolRoot = new GameObject($"{scriptableObject.Identity}_Pool").transform;
            poolRoot.localPosition = Vector3.zero;
            poolRoot.SetParent(_objectPoolRoot);
            
            return poolRoot;
        }

        private IEntity CreatePoolEntity(EntityScriptableObject scriptableObject, Transform parent)
        {
            var prefab     = scriptableObject.MonoEntityPrefab;
            var gameObject = GameObject.Instantiate(prefab, parent, true);
            
            gameObject.gameObject.SetActive(false);
            
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = quaternion.identity;
            gameObject.transform.localScale    = new Vector3(1, 1, 1);

            return null;
        }

        private void OnGetPoolEntity(IEntity entity)
        {
            var monoEntity = entity as MonoEntity;
            
            monoEntity.gameObject.SetActive(true);
            monoEntity.transform.SetParent(null);
        }

        private void OnReleasePoolEntity(IEntity entity)
        {
            var monoEntity = entity as MonoEntity;
            
            monoEntity.gameObject.SetActive(false);
            monoEntity.transform.SetParent(_objectPoolRoot.Find($"{monoEntity.Identity}_Pool").transform);
            
            monoEntity.transform.localPosition = Vector3.zero;
            monoEntity.transform.localRotation = quaternion.identity;
            monoEntity.transform.localScale    = new Vector3(1, 1, 1);
        }

        private void OnDestroyPoolEntity(IEntity entity)
        {
            
        }

        public async UniTaskVoid Emit(EmitInformation emitInformation, CancellationToken cancellationToken = default)
        {
            var element    = _entitiesCollection.Get(emitInformation.Identity);
            var entity = (IEntity)null;

            if (element.UsePool)
                entity = ObtainGameObject(element.EntityScriptableObject);
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

        private IEntity ObtainGameObject(EntityScriptableObject scriptableObject)
        {
            if (_objectPool.TryGetValue(scriptableObject.Identity, out ObjectPool<IEntity> objectPool))
            {
                var entity = objectPool.Get();
                
                return entity;
            }

            throw new Exception($"Cannot obtain entity {scriptableObject.Identity}.");
        }

        private async UniTask<IEntity> CreateEntity(EntityScriptableObject scriptableObject)
        {
            IEntity entity;
            
            if (scriptableObject.Type is EntityScriptableObject.EntityType.Mono)
            {
                var result = await UnityEngine.Object.InstantiateAsync(scriptableObject.MonoEntityPrefab);

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