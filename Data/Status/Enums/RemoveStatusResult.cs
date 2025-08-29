using Systems.SimpleEntities.Data.Status.Abstract;

namespace Systems.SimpleEntities.Data.Status.Enums
{
    /// <summary>
    ///     Status removal result
    /// </summary>
    /// <remarks>
    ///     Order of enum members is important and should follow the order of
    ///     removal operation checks.
    /// </remarks>
    public enum RemoveStatusResult
    {
        /// <summary>
        ///     When status cannot be found in database
        /// </summary>
        InvalidStatus,
        
        /// <summary>
        ///     When status is not applied to entity
        /// </summary>
        NotApplied,
        
        /// <summary>
        ///     When status cannot be removed from entity for some reason
        ///     See <see cref="StatusBase.CanRemove"/>
        /// </summary>
        NotAllowed,
        
        /// <summary>
        ///     When status stack is 0
        /// </summary>
        NotEnoughStacks,
        
        /// <summary>
        ///     When status is removed successfully
        /// </summary>
        Success
    }
}