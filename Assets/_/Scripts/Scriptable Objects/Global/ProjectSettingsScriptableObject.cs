using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Global
{
    public class ProjectSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private string _version;
        [SerializeField] private int _loadingSceneBuildIndex;

        public string Version                => _version;
        public int    LoadingSceneBuildIndex => _loadingSceneBuildIndex;

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