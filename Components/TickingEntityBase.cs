using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Timing;
using Systems.SimpleEntities.Data.Enums;
using Systems.SimpleEntities.Operations;
using Unity.Mathematics;
using UnityEngine;

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