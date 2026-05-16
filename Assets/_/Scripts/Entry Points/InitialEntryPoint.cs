using System.Threading;
using _.Scripts.Scriptable_Objects.Global;
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
        ProjectSettingsScriptableObject _projectSettings;
        ServiceLocator _serviceLocator;

        public InitialEntryPoint(IObjectResolver resolver)
        {
            _projectSettings = resolver.Resolve<ProjectSettingsScriptableObject>();
            _serviceLocator = resolver.Resolve<ServiceLocator>();
        }
        
        public void Start()
        {
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
            
            _serviceLocator.Add(new ScriptableObjectService(), "SO Service");
            
            var scriptableObjectsScene = _serviceLocator.Get<ScriptableObjectService>();
            foreach (var scriptableObject in _projectSettings.ScriptableObjectsToCache)
                scriptableObjectsScene.Add(scriptableObject, scriptableObject.name);
            
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
            
            _serviceLocator.Add(new SceneService(), "Scene Service");
            
            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");
            
            _serviceLocator.Add(new UserInterfaceService(), "UI Service");

            Debug.LogWarning($"ServiceLocator: {(_serviceLocator == null ? "UNDEFINED" : _serviceLocator.ToString())}");

        }

        public void PostStart()
        {
            LoadGameplayScene();
        }
        
        private async UniTaskVoid LoadGameplayScene()
        {
            var sceneService = _serviceLocator.Get<SceneService>();
            var transition = SceneService.SceneTransition
                .Create()
                .WithBuildIndex(_projectSettings.GameplaySceneBuildIndex)
                .WithUseOverlay(true)
                .WithUnloadCurrent()
                .WithDuration(0.1f);

            await UniTask
                .WaitUntil(
                    () => sceneService.IsInitialized, 
                    cancellationToken: Application.exitCancellationToken)
                .SuppressCancellationThrow();
            
            sceneService.Perform(transition, Application.exitCancellationToken);
        }
    }
}