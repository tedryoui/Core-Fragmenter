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
        
        [Serializable]
        public struct BlueprintEvent
        {
            [Serializable, InlineProperty, HideLabel]
            public struct BlueprintEventData
            {
                public enum BlueprintEventDataType
                {
                    INT,
                    STRING,
                    FLOAT,
                    BOOL,
                    OBJECT
                }
                
                [SerializeField] private BlueprintEventDataType _dataType;
                
                [ShowIf("@this._dataType == BlueprintEventDataType.INT")]
                [SerializeField] private int    _intValue;
                [ShowIf("@this._dataType == BlueprintEventDataType.FLOAT")]
                [SerializeField] private float  _floatValue;
                [ShowIf("@this._dataType == BlueprintEventDataType.BOOL")]
                [SerializeField] private bool   _boolValue;
                [ShowIf("@this._dataType == BlueprintEventDataType.STRING")]
                [SerializeField] private string _stringValue;
                [ShowIf("@this._dataType == BlueprintEventDataType.OBJECT")]
                [SerializeField] private Object _objectValue;

                public object Data => _dataType switch
                {
                    BlueprintEventDataType.INT    => _intValue,
                    BlueprintEventDataType.STRING => _stringValue,
                    BlueprintEventDataType.FLOAT  => _floatValue,
                    BlueprintEventDataType.BOOL   => _boolValue,
                    BlueprintEventDataType.OBJECT => _objectValue,
                    _                             => throw new ArgumentOutOfRangeException()
                };
            }
            
            public EventFabric.EventType EventType;
            public BlueprintEventData[]  Data;
        }
        
        [SerializeField] private string _identity;

        [SerializeField] private string _title;
        [SerializeField] private string _baseCost;
        
        [SerializeField] private BlueprintProcess _blueprintProcess;
        
        [ShowIf("@this._blueprintProcess == BlueprintProcess.Delayed")]
        [SerializeField] private float _blueprintDuration;
        
        [SerializeField] private List<BlueprintListing> _inputResources;
        
        [SerializeField] private List<BlueprintEvent> _outputEvents;

        public string Identity => _identity;

        public string Title => _title;


        public BlueprintProcess Process => _blueprintProcess;

        public float BlueprintDuration => _blueprintDuration;

        public IReadOnlyCollection<BlueprintListing> InputResources => _inputResources.AsReadOnly();

        public IReadOnlyCollection<BlueprintEvent> OutputEvents => _outputEvents.AsReadOnly();
    }
}