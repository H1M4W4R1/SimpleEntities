using Systems.SimpleStats.Examples;
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
            statModifiers.Add(new FlatAddModifier<ExampleColdResistanceStatistic>(1f));
            base.RefreshModifiersIfNecessary();
        }
        
    }
}