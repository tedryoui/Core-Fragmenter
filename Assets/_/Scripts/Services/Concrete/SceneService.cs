using System;
using System.Threading;
using _.Scripts.Scriptable_Objects.Global;
using _.Scripts.User_Interface;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _.Scripts.Services
{
    public class SceneService : IService
    {
        public class SceneTransition
        {
            private int   _buildIndex;
            private bool  _useOverlay;
            private float _duration;
            private int   _unloadBuildIndex;

            public int BuildIndex => _buildIndex;
            public bool UseOverlay => _useOverlay;
            public float Duration => _duration;
            public int  UnloadBuildIndex => _unloadBuildIndex;

            private SceneTransition() { }
            
            public static SceneTransition Create()
            {
                return new SceneTransition
                {
                    _buildIndex       = 0,
                    _useOverlay       = true,
                    _duration         = 2f,
                    _unloadBuildIndex = SceneManager.GetActiveScene().buildIndex
                };
            }

            public SceneTransition WithBuildIndex(int buildIndex)
            {
                this._buildIndex = buildIndex;
                return this;
            }

            public SceneTransition WithUseOverlay(bool useOverlay)
            {
                this._useOverlay = useOverlay;
                return this;
            }

            public SceneTransition WithDuration(float duration)
            {
                this._duration = duration;
                return this;
            }

            public SceneTransition WithUnloadBuildIndex(uint unloadBuildIndex)
            {
                this._unloadBuildIndex = (int)unloadBuildIndex;
                return this;
            }

            public SceneTransition WithUnloadCurrent()
            {
                this._unloadBuildIndex = SceneManager.GetActiveScene().buildIndex;
                return this;
            }
        }

        [Inject] private ServiceLocator                  _serviceLocator;
        [Inject] private ProjectSettingsScriptableObject _projectSettings;

        private bool                    _isInitialized;
        private CancellationTokenSource _cancellationTokenSource;

        public bool                    IsInitialized           => _isInitialized;
        public CancellationTokenSource CancellationTokenSource => _cancellationTokenSource;

        public SceneService()
        {
            _cancellationTokenSource = null;
            _isInitialized           = false;
        }

        private async UniTaskVoid WaitUntilLoadingScreenViewModelRegistered(CancellationToken cancellationToken = default, Action onComplete = null)
        {
            if (_serviceLocator is null)
                await UniTask
                    .WaitUntil(
                        () => _serviceLocator != null,
                        cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
            
            if (!_serviceLocator.Has<UserInterfaceService>())
                await UniTask
                    .WaitUntil(
                        () => _serviceLocator.Has<UserInterfaceService>(),
                        cancellationToken: cancellationToken)
                    .SuppressCancellationThrow();
           
            var userInterfaceService = _serviceLocator.Get<UserInterfaceService>();
            await UniTask
                .WaitUntil(
                    () => userInterfaceService.Has<LoadingScreenViewModel>(), 
                    cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            onComplete?.Invoke();
        }
        
        public void Initialize()
        {
            var loadingSceneBuildIndex = _projectSettings.LoadingSceneBuildIndex;
            SceneManager.LoadScene(loadingSceneBuildIndex, LoadSceneMode.Additive);
            
            WaitUntilLoadingScreenViewModelRegistered(
                Application.exitCancellationToken,
                () =>
                {
                    _isInitialized = true;
                    Debug.Log($"<color=green>{nameof(SceneService)} initialized!</color>");
                })
            .Forget();
        }

        public void Dispose()
        {
            
        }

        public void Perform(SceneTransition transition, CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            PerformAsync(transition, _cancellationTokenSource.Token).SuppressCancellationThrow();
        }

        public void Interrupt()
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                throw new Exception("Scene interruption can`t being requested!");
            
            _cancellationTokenSource.Cancel();
        }

        private async UniTask PerformAsync(SceneTransition transition, CancellationToken cancellationToken = default)
        {
            var buildIndex           = transition.BuildIndex;
            var unloadBuildIndex     = transition.UnloadBuildIndex;
            var duration             = transition.Duration;
            
            await ShowOverlay(cancellationToken);

            await LoadSceneAsync(buildIndex, cancellationToken).SuppressCancellationThrow();

            await UniTask
                .Delay(
                    TimeSpan.FromSeconds(duration),
                    cancellationToken: cancellationToken,
                    cancelImmediately: true,
                    ignoreTimeScale: true)
                .SuppressCancellationThrow();
            
            await UnloadSceneAsync(unloadBuildIndex, cancellationToken).SuppressCancellationThrow();

            await HideOverlay(cancellationToken);
        }

        private async UniTask LoadSceneAsync(int buildIndex, CancellationToken cancellationToken = default)
        {
            AsyncOperation operation  = null;
            operation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);

            operation.allowSceneActivation = false;
            await UniTask
                .WaitUntil(
                    () => operation.progress >= 0.9f,
                    cancellationToken: cancellationToken)
                .SuppressCancellationThrow();

            operation.allowSceneActivation = true;
            await UniTask
                .WaitUntil(
                    () => operation.isDone,
                    cancellationToken: cancellationToken)
                .SuppressCancellationThrow();

            Scene loadedScene = SceneManager.GetSceneByBuildIndex(buildIndex);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
                SceneManager.SetActiveScene(loadedScene);
        }

        private async UniTask UnloadSceneAsync(int buildIndex, CancellationToken cancellationToken = default)
        {
            AsyncOperation operation  = null;
            operation = SceneManager.UnloadSceneAsync(buildIndex);

            await UniTask
                .WaitUntil(
                    () => operation.isDone, 
                    cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
            
            operation = Resources.UnloadUnusedAssets();
            await UniTask
                .WaitUntil(
                    () => operation.isDone,
                    cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
        }
        
        private async UniTask ShowOverlay(CancellationToken cancellationToken = default)
        {
            var userInterfaceService       = _serviceLocator.Get<UserInterfaceService>();
            var loadingScreenUserInterface = userInterfaceService.Get<LoadingScreenViewModel>();
            var isCompleted                = false;
            
            loadingScreenUserInterface.Show();
            loadingScreenUserInterface.FadeIn(onComplete: () => isCompleted = true);
            
            await UniTask.WaitUntil(() => isCompleted, cancellationToken: cancellationToken);
        }

        private async UniTask HideOverlay(CancellationToken cancellationToken = default)
        {
            var userInterfaceService       = _serviceLocator.Get<UserInterfaceService>();
            var loadingScreenUserInterface = userInterfaceService.Get<LoadingScreenViewModel>();
            var isCompleted                = false;
            
            loadingScreenUserInterface.FadeOut(onComplete: () =>
            {
                isCompleted = true;
                loadingScreenUserInterface.Hide();
            });
            
            await UniTask.WaitUntil(() => isCompleted, cancellationToken: cancellationToken);
        }
    }
}