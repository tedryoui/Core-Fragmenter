namespace _.Scripts.Gameplay.Entity.Concrete.Core
{
    public class CoreWeakState : AbstractState<CoreEntity>
    {
        public override string Identity => "Core Weak";

        public CoreWeakState(CoreEntity entity) : base(entity)
        {
        }
    }
}