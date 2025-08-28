using JetBrains.Annotations;
using Systems.SimpleEntities.Data.Affinity;

namespace Systems.SimpleEntities.Data.Resistances.Markers
{
    public interface IResistance<[UsedImplicitly] TAffinityType>
        where TAffinityType : AffinityType
    {
        
    }
}