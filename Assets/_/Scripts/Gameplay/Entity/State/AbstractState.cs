namespace @_.Scripts.Gameplay.Entity
{
    public abstract class AbstractState
    {
        public virtual string Identity => "";

        public virtual void OnEnter()  {}
        public virtual void OnUpdate() {}
        public virtual void OnExit()   {}
        
        public override string ToString()
        {
            return $"{Identity} state.";
        }
    }

    public abstract class AbstractState<T> : AbstractState
    where T : IEntity
    {
        private T _entity;
        
        public T Entity => _entity;

        public AbstractState(T entity)
        {
            _entity = entity;
        }
        
        public override string ToString()
        {
            return $"{Identity} state of [HASH: {Entity.GetHashCode()}].";
        }
    }
}