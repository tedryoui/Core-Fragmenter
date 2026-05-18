using _.Scripts.Data.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Concrete.Entities
{
    [CreateAssetMenu(fileName = "Core Entity", menuName = "Core Fragmenter/Entities/Core", order = 1)]
    public class CoreEntityScriptableObject : EntityScriptableObject
    {
        [FoldoutGroup("Data Preset")]
        [SerializeField] private CoreDataPreset _dataPreset;
        
        public CoreDataPreset DataPreset => _dataPreset;
    }
}