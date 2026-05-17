using System.Collections.Generic;
using _.Scripts.Gameplay.Entity.Concrete.Core;

namespace _.Scripts.Gameplay.Entity.Concrete
{
    public class CoreEntity : MonoEntity
    {
        public override string              Identity       => "Core";
        
        public override List<AbstractState> PossibleStates => new List<AbstractState>()
        {
            new CoreWeakState(this),
            new CoreRadiationState(this)
        };
    }
}