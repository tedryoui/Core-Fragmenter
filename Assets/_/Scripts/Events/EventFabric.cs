using System;
using Core.Scripts.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = System.Object;

namespace _.Scripts.Events
{
    public static class EventFabric
    {
        [Serializable]
        public struct Event 
        {
            [Serializable, InlineProperty, HideLabel]
            public struct EventArguments
            {
                public enum EventDataType
                {
                    INT,
                    STRING,
                    FLOAT,
                    BOOL,
                    OBJECT
                }
                
                [SerializeField] private EventDataType _dataType;
                
                [ShowIf("@this._dataType == EventDataType.INT")]
                [SerializeField] private int    _intValue;
                [ShowIf("@this._dataType == EventDataType.FLOAT")]
                [SerializeField] private float  _floatValue;
                [ShowIf("@this._dataType == EventDataType.BOOL")]
                [SerializeField] private bool   _boolValue;
                [ShowIf("@this._dataType == EventDataType.STRING")]
                [SerializeField] private string _stringValue;
                [ShowIf("@this._dataType == EventDataType.OBJECT")]
                [SerializeField] private Object _objectValue;

                public object Data => _dataType switch
                {
                    EventDataType.INT    => _intValue,
                    EventDataType.STRING => _stringValue,
                    EventDataType.FLOAT  => _floatValue,
                    EventDataType.BOOL   => _boolValue,
                    EventDataType.OBJECT => _objectValue,
                    _                    => throw new ArgumentOutOfRangeException()
                };
            }
            
            public EventFabric.EventType EventType;
            public EventArguments[]  Data;
        }
        
        public enum EventType
        {
            AddBlueprint,
            UnlockBlueprint,
            RemoveBlueprint,
            
            AddDrone,
        }

        public static EventBus.IEvent Build(EventType eventType, object[] arguments)
        {
            switch (eventType)
            {
                case EventType.AddBlueprint:
                    return Activator.CreateInstance(typeof(AddBlueprintEvent), args: arguments) as EventBus.IEvent;
                case EventType.UnlockBlueprint:
                    return Activator.CreateInstance(typeof(UnlockBlueprintEvent), args: arguments) as EventBus.IEvent;
                case EventType.RemoveBlueprint:
                    return Activator.CreateInstance(typeof(RemoveBlueprintEvent), args: arguments) as EventBus.IEvent;
                case EventType.AddDrone:
                    return Activator.CreateInstance(typeof(AddDroneEvent), args: arguments) as EventBus.IEvent;
            }
            
            throw new ArgumentOutOfRangeException($"Unknown event {eventType}");
        }

        public static void AnonymousPublish(EventType eventType, EventBus.IEvent @event)
        {
            switch (eventType)
            {
                case EventType.AddBlueprint:
                    EventBus.Instance.Publish<AddBlueprintEvent>(@event as AddBlueprintEvent);
                    break;
                case EventType.UnlockBlueprint:
                    EventBus.Instance.Publish<UnlockBlueprintEvent>(@event as UnlockBlueprintEvent);
                    break;
                case EventType.RemoveBlueprint:
                    EventBus.Instance.Publish<RemoveBlueprintEvent>(@event as RemoveBlueprintEvent);
                    break;
                case EventType.AddDrone:
                    EventBus.Instance.Publish<AddDroneEvent>(@event as AddDroneEvent);
                    break;
                default:
                    throw new ArgumentException($"Unknown event {eventType}");
            }
        }
    }
}