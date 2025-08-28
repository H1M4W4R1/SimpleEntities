using JetBrains.Annotations;
using Systems.SimpleCore.Storage;
using Systems.SimpleEntities.Data.Status.Abstract;

namespace Systems.SimpleEntities.Data
{
    /// <summary>
    ///     Database with all status effects available in game
    /// </summary>
    public sealed class StatusDatabase : AddressableDatabase<StatusDatabase, StatusBase>
    {
        [NotNull] protected override string AddressableLabel => "SimpleEntities.Status";
    }
}