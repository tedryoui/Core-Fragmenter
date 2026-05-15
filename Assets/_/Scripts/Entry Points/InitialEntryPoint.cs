using System.Threading;
using _.Scripts.Services;
using _.Scripts.User_Interface;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _.Scripts.Entry_Points
{
    public class InitialEntryPoint : IStartable, IPostStartable
    {
        ServiceLocator _serviceLocator;

        public InitialEntryPoint(IObjectResolver resolver)
        {
            _serviceLocator = resolver.Resolve<ServiceLocator>();
        }
        
        public void Start()
        {
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
            
            _serviceLocator.Add(new ScriptableObjectService(), "SO Service");
            
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
            
            _serviceLocator.Add(new SceneService(), "Scene Service");
            
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
            
            _serviceLocator.Add(new UserInterfaceService(), "UI Service");

            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");

        }

        public void PostStart()
        {
            RunTransition();
        }

        private async UniTaskVoid RunTransition(CancellationToken cancellationToken = default)
        {
            var userInterfaceService   = _serviceLocator.Get<UserInterfaceService>();

            await UniTask.WaitUntil(() => userInterfaceService.Has<LoadingScreenViewModel>(),
                cancellationToken: cancellationToken);
            
            var loadingScreenViewModel = userInterfaceService.Get<LoadingScreenViewModel>();
            
            loadingScreenViewModel.FadeIn(isInstant: true);

            await UniTask
                .Delay(5000, cancellationToken: cancellationToken, ignoreTimeScale: true, cancelImmediately: true)
                .SuppressCancellationThrow();
            
            loadingScreenViewModel.FadeOut(onComplete: () => {Debug.Log("Fade Completed!");});
        }
    }
}