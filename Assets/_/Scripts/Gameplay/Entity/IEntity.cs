using System.Collections.Generic;

namespace _.Scripts.Gameplay.Entity
{
    public interface IEntity
    {
        public List<AbstractState> PossibleStates { get; }

        public string                            Identity             { get; }
        public string                            DefaultStateIdentity { get; }
        public AbstractState                     CurrentState         { get; }
        public Dictionary<string, AbstractState> StateDictionary      { get; }

        public void SetState(string identity);
    }
}