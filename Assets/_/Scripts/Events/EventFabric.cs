using System;
using Core.Scripts.Helpers;

namespace _.Scripts.Events
{
    public static class EventFabric
    {
        public enum EventType
        {
            AddBlueprint,
            UnlockBlueprint,
            RemoveBlueprint
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
                default:
                    throw new ArgumentException($"Unknown event {eventType}");
            }
        }
    }
}