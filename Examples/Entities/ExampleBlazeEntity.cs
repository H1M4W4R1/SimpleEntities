using Systems.SimpleStats.Examples;
using Systems.SimpleStats.Implementations;

namespace Systems.SimpleEntities.Examples.Entities
{
    /// <summary>
    ///     Entity with fire resistance
    /// </summary>
    public sealed class ExampleBlazeEntity : ExampleEntityBase
    {
        public override void RefreshModifiersIfNecessary()
        {
            statModifiers.Clear();
            statModifiers.Add(new FlatAddModifier<ExampleFireResistanceStatistic>(1f));
            base.RefreshModifiersIfNecessary();
        }
    }
}