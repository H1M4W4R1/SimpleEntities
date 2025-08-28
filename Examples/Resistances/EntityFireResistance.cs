using Systems.SimpleEntities.Examples.Affinity;
using Systems.SimpleEntities.Resistances;
using Systems.SimpleEntities.Resistances.Markers;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Resistances
{
    [CreateAssetMenu(fileName = "EntityFireResistance", menuName = "SimpleEntities/Examples/Resistances/EntityFireResistance")]
    public sealed class EntityFireResistance : ResistanceBase, IResistance<FireAffinity>
    {
   
    }
}