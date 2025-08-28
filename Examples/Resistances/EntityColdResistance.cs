using Systems.SimpleEntities.Examples.Affinity;
using Systems.SimpleEntities.Resistances;
using Systems.SimpleEntities.Resistances.Markers;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Resistances
{
    [CreateAssetMenu(fileName = "EntityColdResistance", menuName = "SimpleEntities/Examples/Resistances/EntityColdResistance")]
    public sealed class EntityColdResistance : ResistanceBase, IResistance<ColdAffinity>
    {
       
    }
}