using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _.Scripts.Gameplay.Entity
{
    public abstract class MonoEntity : MonoBehaviour, IEntity
    {
#if UNITY_EDITOR
        public ValueDropdownList<string> FriendlyPossibleStatesList()
        {
            var list = new ValueDropdownList<string>();

            foreach (var item in PossibleStates)
                list.Add(item.Identity);

            return list;
        }
#endif
        
        [ValueDropdown("FriendlyPossibleStatesList", DropdownTitle = "Select State", FlattenTreeView = true, IsUniqueList = true)]
        [SerializeField] private string _defaultStateIdentity;
        private AbstractState _currentState;
        private Dictionary<string, AbstractState> _stateDictionary;
        
        public abstract List<AbstractState>               PossibleStates       { get; }

        public abstract string        Identity             { get; }
        public          string        DefaultStateIdentity => _defaultStateIdentity;
        public          AbstractState CurrentState         =>  _currentState;
        public Dictionary<string, AbstractState> StateDictionary
        {
            get
            {
                if (_stateDictionary == null)
                    _stateDictionary = PossibleStates.ToDictionary(k => k.Identity, v => v);
                
                return _stateDictionary;
            }
        }

        public virtual void Start()
        {
            SetState(DefaultStateIdentity);
        }

        public void                              SetState(string identity)
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