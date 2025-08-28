using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Data.Status;

namespace Systems.SimpleEntities.Data.Context
{
    public readonly ref struct StatusContext
    {
        public readonly EntityBase entity;
        
        public readonly StatusBase status;
        
        /// <summary>
        ///     Stack count or changed amount
        /// </summary>
        /// <remarks>
        ///     For apply and remove status it returns new stack count.
        ///     In case of status stack changed it returns changed amount with sign.
        /// </remarks>
        public readonly int stackCountOrChange;

        public StatusContext(EntityBase entity, StatusBase status, int stackCountOrChange)
        {
            this.entity = entity;
            this.status = status;
            this.stackCountOrChange = stackCountOrChange;
        }
    }
}