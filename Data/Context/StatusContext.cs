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
        [NotNull] public readonly EntityBase entity;
        
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
        public readonly int stackCountOrChange;

        public StatusContext([NotNull] EntityBase entity, [NotNull] StatusBase status, int stackCountOrChange)
        {
            this.entity = entity;
            this.status = status;
            this.stackCountOrChange = stackCountOrChange;
        }
    }
}