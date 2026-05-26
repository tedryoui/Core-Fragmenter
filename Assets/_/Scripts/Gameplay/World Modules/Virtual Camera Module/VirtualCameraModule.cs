using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _.Scripts.Gameplay.Utility;
using _.Scripts.Scriptable_Objects.Global;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace _.Scripts.Gameplay.World_Modules.Camera_Managing_Module
{
    public class VirtualCameraModule
    {
        [Inject] private ProjectSettingsScriptableObject _projectSettings;
        
        public enum VirtualCameraDefinition
        {
            None,
            Player,
            Notion,
            
            
            Custom = 1000
        }

        private Transform                                              _root;
        private Dictionary<VirtualCameraDefinition, CinemachineCamera> _camerasCache;

        public IReadOnlyDictionary<VirtualCameraDefinition, CinemachineCamera> CamerasCache => _camerasCache;

        private CinemachineBlenderSettings _temporaryBlenderSettings;
        private bool                       _isObserving;
        
        public VirtualCameraModule()
        {
            _camerasCache             = new Dictionary<VirtualCameraDefinition, CinemachineCamera>();
            _root                     = null;
            _isObserving              = false;
            _activeCameraDefinition   = VirtualCameraDefinition.None;
            _temporaryBlenderSettings = ScriptableObject.CreateInstance<CinemachineBlenderSettings>();
        }

        [Inject]
        private void Configure()
        {
            CreateCamerasHolder();
            TryCreateCameras();
        }

        public void RegisterPlayerCamera(CinemachineCamera camera)
        {
            if (_camerasCache.TryAdd(VirtualCameraDefinition.Player, camera))
            {
                camera.Prioritize();

                _activeCameraDefinition = VirtualCameraDefinition.Player;
            }
            else
            {
                Debug.LogWarning($"{VirtualCameraDefinition.Player} is already registered!");
            }
        }

        private void CreateCamerasHolder()
        {
            _root = new GameObject("Camera Root").transform;

            var moduleDebugger = _root.AddComponent<VirtualCameraModule_Debugger>();
            moduleDebugger.module = this;
        }

        private void TryCreateCameras()
        {
            var definitions = (VirtualCameraDefinition[])Enum.GetValues(typeof(VirtualCameraDefinition));
            definitions = definitions.Skip(2).SkipLast(1).ToArray();

            foreach (var definition in definitions)
            {
                var register = _projectSettings.CameraRegisters.FirstOrDefault(x => x.Definition.Equals(definition));

                if (register.Camera != null)
                {
                    var camera = Object.Instantiate(register.Camera);

                    if (!_camerasCache.TryAdd(definition, camera))
                        Debug.LogWarning($"{definition} is already registered!");
                } 
            }
        }

        public void Locate(VirtualCameraDefinition definition, float3 position)
        {
            if (_camerasCache.TryGetValue(definition, out var camera))
            {
                camera.transform.position = position;
            }
            else
            {
                Debug.LogError($"{definition} is not registered!");
            }
        }

        public async UniTaskVoid Observe(VirtualCameraDefinition definition, float duration, bool isForced = false, CinemachineBlenderSettings.CustomBlend[] customBlends = null, CancellationToken cancellationToken = default)
        {
            var brain                  = CinemachineBrain.GetActiveBrain(0);
            var initialBlenderSettings = brain.CustomBlends;
            
            if ((brain.IsBlending || _isObserving) && !isForced)
            {
                Debug.Log($"Brain is blending, cannot observe {definition}!");
                return;    
            }

            var activeCamera = ActiveCameraDefinition;
            
            if (_camerasCache.TryGetValue(definition, out var camera))
            {
                camera.Prioritize();
                _isObserving = true;

                if (customBlends != null)
                {
                    _temporaryBlenderSettings.CustomBlends = customBlends;
                    brain.CustomBlends                     = _temporaryBlenderSettings;
                }

                await UniTask.WaitForEndOfFrame(cancellationToken).SuppressCancellationThrow();

                await UniTask.WaitUntil(
                        () => !brain.IsBlending,
                        cancellationToken: cancellationToken,
                        cancelImmediately: true)
                    .SuppressCancellationThrow();

                await UniTask
                    .Delay(
                        TimeSpan.FromSeconds(duration), 
                        cancellationToken: cancellationToken,
                        cancelImmediately: true)
                    .SuppressCancellationThrow();

                if (_camerasCache.TryGetValue(activeCamera, out var previousCamera))
                {
                    previousCamera.Prioritize();

                    if (customBlends != null)
                    {
                        await UniTask.WaitForEndOfFrame(cancellationToken).SuppressCancellationThrow();

                        await UniTask.WaitUntil(
                                () => !brain.IsBlending,
                                cancellationToken: cancellationToken,
                                cancelImmediately: true)
                            .SuppressCancellationThrow();

                        brain.CustomBlends = initialBlenderSettings;
                    }
                    
                    _isObserving = false;
                }
                else
                {
                    Debug.LogError($"{activeCamera} is not registered!");
                }
            }
            else
            {
                Debug.LogError($"{definition} is not registered!");
            }
        }

        public string GetCameraName(VirtualCameraDefinition definition = VirtualCameraDefinition.None)
        {
            if (definition == VirtualCameraDefinition.None)
                return "**ANY CAMERA**";
            
            if (_camerasCache.TryGetValue(definition, out var camera))
                return camera.name;

            throw new KeyNotFoundException($"Camera {definition} not found!");
        }

        private VirtualCameraDefinition _activeCameraDefinition;
        
        public VirtualCameraDefinition ActiveCameraDefinition
        {
            get
            {
                if (_activeCameraDefinition == VirtualCameraDefinition.None)
                {
                    var activeVirtualCamera = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera;

                    var virtualCameraDefinition = _camerasCache.FirstOrDefault(x => x.Value.Equals(activeVirtualCamera)).Key;

                    _activeCameraDefinition = virtualCameraDefinition;
                    
                    Debug.Log("Current active camera definition: " + _activeCameraDefinition);
                }
                
                return _activeCameraDefinition;
            }
        }

    }
}