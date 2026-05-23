using System;
using System.Collections.Generic;
using _.Scripts.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _.Scripts.Scriptable_Objects
{
    [CreateAssetMenu(fileName = "Blueprint", menuName = "Core Fragmenter/Blueprint", order = 0)]
    public class BlueprintScriptableObject : ScriptableObject
    {
        public enum BlueprintProcess
        {
            Instant,
            Delayed
        }

        [Serializable]
        public struct BlueprintListing
        {
            public ResourceConfigScriptableObject ResourceConfig;
            public uint                           Quantity;
        }
        
        [SerializeField] private string _identity;

        [SerializeField] private string _title;
        [SerializeField] private string _baseCost;
        
        [SerializeField] private BlueprintProcess _blueprintProcess;
        
        [ShowIf("@this._blueprintProcess == BlueprintProcess.Delayed")]
        [SerializeField] private float _blueprintDuration;
        
        [SerializeField] private List<BlueprintListing> _inputResources;
        
        [SerializeField] private List<EventFabric.Event> _outputEvents;

        public string Identity => _identity;

        public string Title => _title;


        public BlueprintProcess Process => _blueprintProcess;

        public float BlueprintDuration => _blueprintDuration;

        public IReadOnlyCollection<BlueprintListing> InputResources => _inputResources.AsReadOnly();

        public IReadOnlyCollection<EventFabric.Event> OutputEvents => _outputEvents.AsReadOnly();
    }
}