using System;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Gameplay.Entity;
using _.Scripts.Gameplay.World_Modules;
using _.Scripts.Scriptable_Objects.Global;
using UnityEngine;
using VContainer;

namespace _.Scripts.Services
{
    public class WorldService : IService
    {
        [Inject] private IObjectResolver                 _objectResolver;
        [Inject] private ProjectSettingsScriptableObject _projectSettings;

        private EntityEmittingModule _emittingModule;
        
        private Dictionary<string, IEntity> _entities;
        
        public EntityEmittingModule EntityEmittingModule => _emittingModule;

        public WorldService()
        {
            _entities = new Dictionary<string, IEntity>();
            
            _emittingModule = new EntityEmittingModule();
        }
        
        public void Initialize()
        {
            _objectResolver.Inject(_emittingModule);
            
            Debug.Log($"<color=green>{nameof(WorldService)} initialized!</color>");
        }

        public void Dispose()
        {
            Debug.Log($"<color=red>{nameof(WorldService)} is disposed!</color>");
        }

        public void Register(string identity, IEntity gameObject)
        {
            if (!_entities.TryAdd(identity, gameObject))
                throw new OverflowException($"<color=red>Entity {identity} is already registered!</color>");
        }

        public void Remove(string identity)
        {
            if (!_entities.Remove(identity))
                throw new KeyNotFoundException($"<color=red>Entity {identity} not found!</color>");
        }

        public void Remove(IEntity entity)
        {
            var firstOrDefault = _entities.FirstOrDefault(x => x.Value.Equals(entity));
            
            if (firstOrDefault.Value == null)
                throw new KeyNotFoundException($"<color=red>Entity {entity.Identity} not found!</color>");
            
            _entities.Remove(firstOrDefault.Key);
        }
     }
}