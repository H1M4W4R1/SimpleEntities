using JetBrains.Annotations;
using Systems.SimpleEntities.Affinity;

namespace Systems.SimpleEntities.Resistances.Markers
{
    public interface IResistance<[UsedImplicitly] TAffinityType>
        where TAffinityType : DamageAffinity
    {
        
    }
}