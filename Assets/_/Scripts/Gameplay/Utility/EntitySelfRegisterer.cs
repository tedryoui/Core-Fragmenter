using System;
using _.Scripts.Gameplay.Entity;
using _.Scripts.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _.Scripts.Gameplay.Utility
{
    public class EntitySelfRegisterer : MonoBehaviour
    {
        [SerializeField, DisableIf("@true")] private MonoEntity _entity;

        private void OnValidate()
        {
            if (_entity == null)
                _entity = GetComponent<MonoEntity>();
        }

        private void Start()
        {
            var lifetimeScope = GameObject.FindAnyObjectByType<LifetimeScope>();

            var serviceLocator = lifetimeScope.Container.Resolve<ServiceLocator>();

            var worldService = serviceLocator.Get<WorldService>();
            
            worldService.Register(_entity.Identity, _entity);
        }
    }
}