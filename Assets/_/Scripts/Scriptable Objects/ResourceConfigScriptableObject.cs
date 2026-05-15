using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "CoreFragmenter/Resource Config")]
    public class ResourceConfigScriptableObject : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string _resourceId;
        [SerializeField] private string _resourceName;

        [Header("Presentation")]
        [SerializeField] private Sprite _resourceIcon;

        [Header("Economy")]
        [SerializeField] private bool _isPremium;

        public string ResourceId => _resourceId;
        public string ResourceName => _resourceName;
        public Sprite ResourceIcon => _resourceIcon;
        public bool IsPremium => _isPremium;
    }
}
