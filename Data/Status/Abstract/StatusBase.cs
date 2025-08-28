using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleEntities.Data.Context;
using UnityEngine;

namespace Systems.SimpleEntities.Data.Status.Abstract
{
    /// <summary>
    ///     Represents a status effect that can be applied to entities
    /// </summary>
    [AutoCreatedObject("Status", StatusDatabase.LABEL)]
    public abstract class StatusBase : ScriptableObject
    {
        /// <summary>
        ///     Max stack of status effect
        /// </summary>
        /// <remarks>
        ///     For infinite stack set to -1. When set to 1 it works as active/inactive status.
        ///     It can also support percentages, in such case set to 100, 1K or 10K depending on
        ///     precision you need.
        /// </remarks>
        [field: SerializeField] public virtual int MaxStack { get; private set; }
        
        /// <summary>
        ///     Executed when status is applied to entity for the first time
        /// </summary>
        protected internal virtual void OnStatusApplied(in StatusContext context){}
        
        /// <summary>
        ///     Executed when status is removed from entity (stack reached 0)
        /// </summary>
        protected internal virtual void OnStatusRemoved(in StatusContext context){}
        
        /// <summary>
        ///     Called when status is already applied and stack count changes     
        /// </summary>
        protected internal virtual void OnStatusStackChanged(in StatusContext context){}
        
        /// <summary>
        ///     Called every tick while status is active
        /// </summary>
        protected internal virtual void OnStatusTick(in StatusContext context, float deltaTime){}
    }
}