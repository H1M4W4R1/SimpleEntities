using Systems.SimpleEntities.Data.Status.Abstract;

namespace Systems.SimpleEntities.Data.Status.Enums
{
    /// <summary>
    ///     Status application result
    /// </summary>
    /// <remarks>
    ///     The order of enum members is important and should follow the order of
    ///     status application checks.
    /// </remarks>
    public enum ApplyStatusResult
    {
        /// <summary>
        ///     When status cannot be found in database
        /// </summary>
        InvalidStatus,
        
        /// <summary>
        ///     When status cannot be applied to entity for some reason
        ///     See <see cref="StatusBase.CanApply"/>
        /// </summary>
        NotAllowed,
        
        /// <summary>
        ///     Cannot apply status, because max stack reached
        /// </summary>
        MaxStackReached,
        
        /// <summary>
        ///     Status applied successfully
        /// </summary>
        Success,
    }
}