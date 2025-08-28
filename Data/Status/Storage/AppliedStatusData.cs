using JetBrains.Annotations;

namespace Systems.SimpleEntities.Data.Status
{
    /// <summary>
    ///     Contains data about applied status
    /// </summary>
    public struct AppliedStatusData
    {
        /// <summary>
        ///     Status that is applied
        /// </summary>
        [NotNull] public readonly StatusBase status;
        
        /// <summary>
        ///     Current stack count
        /// </summary>
        public int stackCount;

        public AppliedStatusData([NotNull] StatusBase status, int stackCount)
        {
            this.status = status;
            this.stackCount = stackCount;
        }
    }
}