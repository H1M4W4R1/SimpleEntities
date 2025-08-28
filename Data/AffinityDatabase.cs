using JetBrains.Annotations;
using Systems.SimpleCore.Storage;
using Systems.SimpleEntities.Data.Affinity;

namespace Systems.SimpleEntities.Data
{
    /// <summary>
    ///     Database of all damage affinities in game
    /// </summary>
    public sealed class AffinityDatabase : AddressableDatabase<AffinityDatabase, AffinityType>
    {
        [NotNull] protected override string AddressableLabel => "SimpleEntities.Affinity";
    }
}