using System;
using _.Scripts.Services;
using _.Scripts.User_Interface;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _.Scripts.Utilities
{
    public class UserInterfaceAutoRegisterer : MonoBehaviour
    {
        [SerializeField, SerializeReference] private AbstractUserInterfaceViewModel _abstractUserInterfaceViewModel;

        private void Awake()
        {
            var lifetimeScope = FindAnyObjectByType<LifetimeScope>();

            lifetimeScope.Container.Inject(_abstractUserInterfaceViewModel);
        }
    }
}