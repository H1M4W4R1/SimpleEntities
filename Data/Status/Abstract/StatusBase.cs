using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleCore.Operations;
using Systems.SimpleEntities.Data.Context;
using Systems.SimpleEntities.Operations;
using UnityEngine;

namespace Systems.SimpleEntities.Data.Status.Abstract
{
    /// <summary>
    ///     Represents a status effect that can be applied to entities
    /// </summary>
    [AutoCreate("Status", StatusDatabase.LABEL)] public abstract class StatusBase : ScriptableObject
    {
        /// <summary>
        ///     Max stack of status effect
        /// </summary>
        /// <remarks>
        ///     For infinite stack set to -1. When set to 1 it works as active/inactive status.
        ///     It can also support percentages, in such case set to 100, 1K or 10K depending on
        ///     precision you need.
        /// </remarks>
        [field: SerializeField] public int MaxStack { get; private set; }

        /// <summary>
        ///     Checks if status can be applied to entity.
        /// </summary>
        protected internal virtual OperationResult CanApply(in StatusContext context) => EntityOperations.Permitted();

        /// <summary>
        ///     Checks if status can be removed from entity.
        /// </summary>
        protected internal virtual OperationResult CanRemove(in StatusContext context) => EntityOperations.Permitted();


        /// <summary>
        ///     Executed when status application is failed due to <see cref="CanApply"/> or related reasons
        /// </summary>
        protected internal virtual void OnStatusApplicationFailed(
            in StatusContext context,
            in OperationResult<int> resultExpectedStacks)
        {
        }

        /// <summary>
        ///     Executed when status is applied to entity for the first time
        /// </summary>
        protected internal virtual void OnStatusApplied(
            in StatusContext context,
            in OperationResult<int> resultStackCount)
        {
        }


        /// <summary>
        ///     Executed when status removal is failed due to <see cref="CanRemove"/> or related reasons
        /// </summary>
        protected internal virtual void OnStatusRemovalFailed(
            in StatusContext context,
            in OperationResult<int> resultExpectedStacks)
        {
        }

        /// <summary>
        ///     Executed when status is removed from entity (stack reached 0)
        /// </summary>
        protected internal virtual void OnStatusRemoved(
            in StatusContext context,
            in OperationResult<int> resultStackCount)
        {
        }

        /// <summary>
        ///     Called when status stack is changed
        /// </summary>
        protected internal virtual void OnStatusStackChanged(
            in StatusContext context,
            in OperationResult<int> resultStackCount)
        {
        }

        /// <summary>
        ///     Called every tick while status is active
        /// </summary>
        protected internal virtual void OnStatusTick(in StatusContext context, float deltaTime)
        {
        }
    }
}