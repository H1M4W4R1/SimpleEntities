using Systems.SimpleCore.Operations;
using Systems.SimpleEntities.Data.Context;
using Systems.SimpleEntities.Data.Status.Abstract;
using Systems.SimpleEntities.Operations;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Status
{
    /// <summary>
    ///     Burning status example
    /// </summary>
    public sealed class BurningStatusExample : StatusBase
    {
        protected internal override void OnStatusApplied(
            in StatusContext context,
            in OperationResult result,
            int currentStacks)
        {
            base.OnStatusApplied(in context, result, currentStacks);
            Debug.Log($"{context.entity.name} is burning with {currentStacks} stacks!");
        }

        protected internal override void OnStatusApplicationFailed(
            in StatusContext context,
            in OperationResult result)
        {
            base.OnStatusApplicationFailed(in context, in result);
            if (OperationResult.AreSimilar(result, StatusOperations.MaxStackReached()))
            {
                Debug.Log($"{context.entity.name} is already burning at max stacks!");
            }
        }

        protected internal override void OnStatusStackChanged(
            in StatusContext context,
            in OperationResult result,
            int currentStacks)
        {
            base.OnStatusStackChanged(in context, result, currentStacks);
            Debug.Log($"{context.entity.name} is burning with {currentStacks} stacks!");
        }

        protected internal override void OnStatusRemoved(
            in StatusContext context,
            in OperationResult resultStackCount)
        {
            base.OnStatusRemoved(in context, resultStackCount);
            Debug.Log($"{context.entity.name} is no longer burning!");
        }
    }
}