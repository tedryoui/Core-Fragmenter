using _.Scripts.Data.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Concrete.Entities
{
    [CreateAssetMenu(fileName = "Drone Entity", menuName = "Core Fragmenter/Entities/Drone", order = 1)]
    public class DroneEntityScriptableObject : EntityScriptableObject
    {
        [FoldoutGroup("Data Preset")]
        [SerializeField] private DroneDataPreset _dataPreset;

        public DroneDataPreset DataPreset => _dataPreset;
    }
}