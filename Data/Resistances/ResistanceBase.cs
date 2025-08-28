using System.Runtime.CompilerServices;
using Systems.SimpleEntities.Data.Affinity;
using Systems.SimpleEntities.Data.Resistances.Markers;
using Systems.SimpleStats.Data.Statistics;

namespace Systems.SimpleEntities.Data.Resistances
{
    
    /// <summary>
    ///     Base class for resistance statistics with built-in support
    /// </summary>
    public abstract class ResistanceBase : StatisticBase
    {
        /// <summary>
        ///     Checks if this resistance is valid for the given affinity type
        /// </summary>
        /// <typeparam name="TAffinityType">Affinity type to check</typeparam>
        /// <returns>True if this resistance is valid for the given affinity type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidFor<TAffinityType>()
            where TAffinityType : AffinityType => this is IResistance<TAffinityType>;
    }
}