using Core.Scripts.Helpers;

namespace _.Scripts.Events
{
    public class UnlockBlueprintEvent : EventBus.IEvent
    {
        private string _identity;

        public string Identity => _identity;

        public UnlockBlueprintEvent(string identity)
        {
            _identity = identity;
        }
    }
}