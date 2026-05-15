using _.Scripts.Scriptable_Objects.Global;
using UnityEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    [SerializeField] private ProjectSettingsScriptableObject _projectSettings;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterInstance<ProjectSettingsScriptableObject>(_projectSettings).AsSelf();
        builder.Register<ServiceLocator>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }
    
    #if UNITY_EDITOR
    private void CreateNewProjectSettingsScriptableObject()
    {
        _projectSettings = ScriptableObject.CreateInstance<ProjectSettingsScriptableObject>();
        
        AssetDatabase.CreateAsset(_projectSettings, "Assets/ProjectSettings.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endif
}
