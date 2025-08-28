using Systems.SimpleEntities.Data.Context;
using Systems.SimpleEntities.Data.Status.Abstract;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Status
{
    /// <summary>
    ///     Burning status example
    /// </summary>
    public sealed class BurningStatusExample : StatusBase
    {
        public override int MaxStack => 5;

        protected internal override void OnStatusApplied(in StatusContext context)
        {
            base.OnStatusApplied(in context);
            Debug.Log($"{context.entity.name} is burning!");
        }

        protected internal override void OnStatusRemoved(in StatusContext context)
        {
            base.OnStatusRemoved(in context);
            Debug.Log($"{context.entity.name} is no longer burning!");
        }
    }
}