using Systems.SimpleEntities.Data.Resistances;
using Systems.SimpleEntities.Data.Resistances.Markers;
using Systems.SimpleEntities.Examples.Affinity;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Resistances
{
    [CreateAssetMenu(fileName = "EntityColdResistance", menuName = "SimpleEntities/Examples/Resistances/EntityColdResistance")]
    public sealed class EntityColdResistance : ResistanceBase, IResistance<ColdAffinity>
    {
       
    }
}