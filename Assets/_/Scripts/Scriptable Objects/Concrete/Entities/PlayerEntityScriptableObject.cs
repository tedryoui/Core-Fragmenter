using _.Scripts.Data.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects.Concrete.Entities
{
    [CreateAssetMenu(fileName = "Player Entity", menuName = "Core Fragmenter/Entities/Player Entity", order = 0)]
    public class PlayerEntityScriptableObject : EntityScriptableObject
    {
        [FoldoutGroup("Data Preset")]
        [SerializeField] private PlayerDataPreset _dataPreset;

        public PlayerDataPreset DataPreset => _dataPreset;
    }
}