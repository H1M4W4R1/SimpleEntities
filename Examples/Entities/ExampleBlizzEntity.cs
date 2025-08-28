using Systems.SimpleEntities.Examples.Resistances;
using Systems.SimpleStats.Implementations;

namespace Systems.SimpleEntities.Examples.Entities
{
    /// <summary>
    ///     Entity with cold resistance
    /// </summary>
    public sealed class ExampleBlizzEntity : ExampleEntityBase
    {
        public override void RefreshModifiersIfNecessary()
        {
            statModifiers.Clear();
            statModifiers.Add(new FlatAddModifier<EntityColdResistance>(1f));
            base.RefreshModifiersIfNecessary();
        }
        
    }
}