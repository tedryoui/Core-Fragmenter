using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _.Scripts.Gameplay.World_Modules.Camera_Managing_Module;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace _.Scripts.Gameplay.Utility
{
    public class VirtualCameraModule_Debugger : MonoBehaviour
    {
        [ReadOnly] public VirtualCameraModule module;

        [ReadOnly, Serializable, InlineProperty, HideLabel]
        public struct CameraDebugEntry
        {
            public VirtualCameraModule.VirtualCameraDefinition Definition;
            public CinemachineCamera                           Camera;
        }

        [SerializeField, ReadOnly, ListDrawerSettings(ShowFoldout = false)] private List<CameraDebugEntry> _cameraDebugEntries;
        
        [SerializeField, ReadOnly] private VirtualCameraModule.VirtualCameraDefinition _activeCameraDefinition;
        [SerializeField, ReadOnly] private bool                                        _isBlending;
        
        private CancellationTokenSource _cancellationTokenSource;
        
        [Button]
        private void Observe(VirtualCameraModule.VirtualCameraDefinition activeCameraDefinition, float duration = 1f)
        {
            _cancellationTokenSource = CancellationTokenSource
                .CreateLinkedTokenSource(Application.exitCancellationToken);

            module.Observe(
                activeCameraDefinition,
                duration,
                customBlends: new[]
                {
                    new CinemachineBlenderSettings.CustomBlend
                    {
                        From  = module.GetCameraName(VirtualCameraModule.VirtualCameraDefinition.Player),
                        To    = module.GetCameraName(VirtualCameraModule.VirtualCameraDefinition.Notion),
                        Blend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 0.25f)
                    },
                    new CinemachineBlenderSettings.CustomBlend()
                    {
                        From  = module.GetCameraName(VirtualCameraModule.VirtualCameraDefinition.Notion),
                        To    = module.GetCameraName(VirtualCameraModule.VirtualCameraDefinition.Player),
                        Blend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.HardIn, 0.50f)
                    }
                },
                cancellationToken: _cancellationTokenSource.Token);
        }

        [Button]
        private void Cancel()
        {
            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();
        }
        
        private void Update()
        {
            _activeCameraDefinition = module.ActiveCameraDefinition;

            _cameraDebugEntries = module.CamerasCache.Select(x => new CameraDebugEntry
            {
                Definition = x.Key,
                Camera     = x.Value
            }).ToList();

            _isBlending = CinemachineBrain.GetActiveBrain(0).IsBlending;
        }
    }
}