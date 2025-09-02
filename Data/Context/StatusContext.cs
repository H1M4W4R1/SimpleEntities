using JetBrains.Annotations;
using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Data.Status.Abstract;

namespace Systems.SimpleEntities.Data.Context
{
    /// <summary>
    ///     Status context for handling all status events, common between apply, remove and stack changed
    /// </summary>
    public readonly ref struct StatusContext
    {
        /// <summary>
        ///     Entity that has the status
        /// </summary>
        [NotNull] public readonly AliveEntityBase entity;
        
        /// <summary>
        ///     Status that is applied to the entity
        /// </summary>
        [NotNull] public readonly StatusBase status;
        
        /// <summary>
        ///     Stack count or changed amount
        /// </summary>
        /// <remarks>
        ///     For apply and remove status it returns new stack count.
        ///     In case of status stack changed it returns changed amount with sign.
        /// </remarks>
        public readonly int currentStackCount;

        public StatusContext([NotNull] AliveEntityBase entity, [NotNull] StatusBase status, int currentStackCount)
        {
            this.entity = entity;
            this.status = status;
            this.currentStackCount = currentStackCount;
        }
    }
}