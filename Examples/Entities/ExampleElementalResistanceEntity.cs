using Systems.SimpleEntities.Data.Resistances;
using Systems.SimpleStats.Implementations;

namespace Systems.SimpleEntities.Examples.Entities
{
    /// <summary>
    ///     Entity with all resistances + 50%
    /// </summary>
    public sealed class ExampleElementalResistanceEntity : ExampleEntityBase
    {
        public override void RefreshModifiersIfNecessary()
        {
            statModifiers.Clear();
            statModifiers.Add(new FlatAddModifier<ResistanceBase>(0.5f));
            base.RefreshModifiersIfNecessary();
        }
    }
}