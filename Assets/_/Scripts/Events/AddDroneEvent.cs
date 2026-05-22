using Core.Scripts.Helpers;
using UnityEngine;

namespace _.Scripts.Events
{
    public class AddDroneEvent : EventBus.IEvent
    {
        private string _scriptableObjectIdentity;
        
        public string ScriptableObjectIdentity => _scriptableObjectIdentity;

        public AddDroneEvent(string scriptableObjectIdentity)
        {
            _scriptableObjectIdentity = scriptableObjectIdentity;
        }
    }
}