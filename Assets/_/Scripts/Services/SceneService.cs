using _.Scripts.Scriptable_Objects.Global;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _.Scripts.Services
{
    public class SceneService : IService
    {
        [Inject] private ProjectSettingsScriptableObject _projectSettings;
        
        public void Initialize()
        {
            var loadingSceneBuildIndex = _projectSettings.LoadingSceneBuildIndex;
            SceneManager.LoadScene(loadingSceneBuildIndex, LoadSceneMode.Additive);
            
            Debug.Log($"<color=green>{nameof(SceneService)} initialized!</color>");
        }

        public void Dispose()
        {
            
        }
    }
}