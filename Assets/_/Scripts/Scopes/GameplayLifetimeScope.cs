using _.Scripts.Entry_Points;
using VContainer;
using VContainer.Unity;

namespace _.Scripts.Scopes
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameplaySceneEntryPoint>();
        }
    }
}