using Systems.SimpleCore.Timing;

namespace Systems.SimpleEntities.Components
{
    public abstract class TickingEntityBase : EntityBase
    {
        protected override void OnEntityActivated()
        {
            TickSystem.EnsureExists();
            base.OnEntityActivated();
            TickSystem.OnTick += OnTick;
        }

        protected override void OnEntityDeactivated()
        {
            base.OnEntityDeactivated();
            TickSystem.OnTick -= OnTick;
        }

        protected virtual void OnTick(float deltaTime)
        {
        }

    }
}