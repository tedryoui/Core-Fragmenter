using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Blueprint", menuName = "Core Fragmenter/Blueprint", order = 0)]
    public class BlueprintScriptableObject : ScriptableObject
    {
        public enum BlueprintType
        {
            Craft,
            Technology,
            Event
        }
        
        public enum BlueprintProcess
        {
            Instant,
            Delayed
        }
        
        [SerializeField] private string _identity;

        [SerializeField] private string _title;
        [SerializeField] private string _baseCost;
        
        [SerializeField] private BlueprintType    _blueprintType;
        [SerializeField] private BlueprintProcess _blueprintProcess;
        
        [ShowIf("@this._blueprintProcess == BlueprintProcess.Delayed")]
        [SerializeField] private float _blueprintDuration;
        
        [SerializeField] private List<ResourceConfigScriptableObject> _inputResources;
        [ShowIf("@this._blueprintType == BlueprintType.Craft")]
        [SerializeField] private ResourceConfigScriptableObject       _outputResource;
        [ShowIf("@this._blueprintType == BlueprintType.Event")]
        [SerializeField] private string _eventIdentity;

        public string Identity => _identity;

        public string Title => _title;

        public BlueprintType Type => _blueprintType;

        public BlueprintProcess Process => _blueprintProcess;

        public float BlueprintDuration => _blueprintDuration;

        public IReadOnlyCollection<ResourceConfigScriptableObject> InputResources => _inputResources;

        public ResourceConfigScriptableObject OutputResource => _outputResource;
    }
}