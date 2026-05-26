using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _.Scripts.Gameplay.World_Modules.Camera_Managing_Module;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Global
{
    public class ProjectSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private string _version;
        
#if UNITY_EDITOR
        [SerializeField] private int _startSceneBuildIndex;

        public void OnStartSceneBuildIndexChanged()
        {
            var sceneEditorSettings = EditorBuildSettings.scenes[_startSceneBuildIndex];
            
            if (!sceneEditorSettings.enabled) return;
            if (string.IsNullOrEmpty(sceneEditorSettings.path)) return;

            var path = sceneEditorSettings.path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            
            EditorSceneManager.playModeStartScene = sceneAsset;
            
            var assetName = path.Split('/').Last();
            var sceneName = assetName.Substring(0, assetName.Length - 6);
            
            Debug.LogWarning($"Updated start scene build index to {_startSceneBuildIndex} [{sceneName}]");
        }
#endif
        
        [SerializeField] private int _loadingSceneBuildIndex;
        [SerializeField] private int _gameplaySceneBuildIndex;

        [SerializeField] private List<ScriptableObject> _scriptableObjectsToCache;

        public string Version                => _version;
        public int    LoadingSceneBuildIndex => _loadingSceneBuildIndex;
        public int    GameplaySceneBuildIndex => _gameplaySceneBuildIndex;
        
        public IReadOnlyList<ScriptableObject> ScriptableObjectsToCache => _scriptableObjectsToCache;
        
        [Serializable]
        public struct CameraRegister
        {
            public VirtualCameraModule.VirtualCameraDefinition Definition;
            public CinemachineCamera                           Camera;
        }
        
        [SerializeField] private List<CameraRegister> _cameraRegisters;
        public IReadOnlyCollection<CameraRegister> CameraRegisters => _cameraRegisters;
        
#if UNITY_EDITOR
        public static IEnumerable FriendlySceneBuildIndexList()
        {
            var list       = new ValueDropdownList<int>();

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scenePath = EditorBuildSettings.scenes[i];

                if (!scenePath.enabled) continue;
                if (string.IsNullOrEmpty(scenePath.path)) continue;

                var assetName = scenePath.path.Split('/').Last();
                var name = assetName.Substring(0, assetName.Length - 6);
                
                list.Add(name, i);
            }

            return list;
        }
#endif
    }
}