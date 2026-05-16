using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity
{
    [Serializable]
    public abstract class AbstractEntity : IEntity
    {
        private string                            _defaultStateIdentity;
        private AbstractState                     _currentState;
        private Dictionary<string, AbstractState> _stateDictionary;
        
        public abstract List<AbstractState>               PossibleStates       { get; }

        public abstract string        Identity             { get; }
        public          string        DefaultStateIdentity => _defaultStateIdentity;
        public          AbstractState CurrentState         => _currentState;

        public Dictionary<string, AbstractState> StateDictionary
        {
            get
            {
                _stateDictionary ??= PossibleStates.ToDictionary(k => k.Identity, v => v);

                return _stateDictionary;
            }
        }

        protected AbstractEntity(string defaultStateIdentity)
        {
            _defaultStateIdentity = defaultStateIdentity;
            
            SetState(defaultStateIdentity);
        }

        public void SetState(string identity)
        {
            if (!StateDictionary.TryGetValue(identity, out AbstractState state))
                throw new KeyNotFoundException($"Entity {Identity} has no state {identity}");
            
            if (CurrentState != null)
                CurrentState.OnExit();
            
            _currentState = state;
            CurrentState.OnEnter();
        }
    }
}