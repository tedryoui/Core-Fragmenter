using Core.Scripts.Helpers;

namespace _.Scripts.Events
{
    public class RemoveBlueprintEvent : EventBus.IEvent
    {
        private string _identity;

        public string Identity => _identity;

        public RemoveBlueprintEvent(string identity)
        {
            _identity = identity;
        }
    }
}