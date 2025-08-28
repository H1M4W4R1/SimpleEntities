using Systems.SimpleEntities.Data.Resistances;
using Systems.SimpleEntities.Data.Resistances.Markers;
using Systems.SimpleEntities.Examples.Affinity;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Resistances
{
    [CreateAssetMenu(fileName = "EntityFireResistance", menuName = "SimpleEntities/Examples/Resistances/EntityFireResistance")]
    public sealed class EntityFireResistance : ResistanceBase, IResistance<FireAffinity>
    {
   
    }
}