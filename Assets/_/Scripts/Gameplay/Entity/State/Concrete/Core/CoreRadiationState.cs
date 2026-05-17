namespace _.Scripts.Gameplay.Entity.Concrete.Core
{
    public class CoreRadiationState : AbstractState<CoreEntity>
    {
        public override string Identity => "Core Radiation";

        public CoreRadiationState(CoreEntity entity) : base(entity)
        {
        }
    }
}