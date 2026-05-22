using Core.Scripts.Helpers;

namespace _.Scripts.Events
{
    public class AddBlueprintEvent : EventBus.IEvent
    {
        private string _identity;
        private bool   _unlocked;

        public string Identity => _identity;
        public bool   Unlocked => _unlocked;

        public AddBlueprintEvent(string identity, bool unlocked = false)
        {
            _identity = identity;
            _unlocked = unlocked;
        }
    }
}