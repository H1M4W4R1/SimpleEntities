using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Data;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Examples;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Entities
{
    /// <summary>
    ///     Crude entity with two potential types of resistance, very bad coding example as
    ///     it's possible to create ResistanceStatisticBase and provide damage affinity to this resistance
    ///     to check if this statistic affects specified type of damage.
    ///
    ///     Generally this is an example how to access statistic and compute value based on modifiers.
    /// </summary>
    public abstract class ExampleEntityBase : EntityBase
    {
        [ContextMenu("Deal fire damage")] public void DealFireDamage()
        {
            RefreshModifiersIfNecessary();
            StatModifierCollection resistanceModifiers = new(GetModifiersFor<ExampleFireResistanceStatistic>());
            ExampleFireResistanceStatistic resistanceStatistic =
                StatsDatabase.GetStatistic<ExampleFireResistanceStatistic>();

            // ReSharper disable once Unity.NoNullPropagation
            float resistanceValue = resistanceStatistic?.GetFinalValue(resistanceModifiers) ?? 0;

            float damage = MaxHealth;
            float finalDamage = damage * math.clamp(1 - resistanceValue, 0, 1);
            
            Damage(new DamageContext(this, (int) finalDamage));
        }

        [ContextMenu("Deal cold damage")] public void DealColdDamage()
        {
            RefreshModifiersIfNecessary();
            StatModifierCollection resistanceModifiers = new(GetModifiersFor<ExampleColdResistanceStatistic>());
            ExampleColdResistanceStatistic resistanceStatistic =
                StatsDatabase.GetStatistic<ExampleColdResistanceStatistic>();

            // ReSharper disable once Unity.NoNullPropagation
            float resistanceValue = resistanceStatistic?.GetFinalValue(resistanceModifiers) ?? 0;
            
            float damage = MaxHealth;
            float finalDamage = damage * math.clamp(1 - resistanceValue, 0, 1);
            
            Damage(new DamageContext(this, (int) finalDamage));
        }
    }
}