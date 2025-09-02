using JetBrains.Annotations;
using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Data.Affinity;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Systems.SimpleEntities.Data.Context
{
    /// <summary>
    ///     Context for damage
    /// </summary>
    public readonly ref struct DamageContext
    {
        /// <summary>
        ///     Damage source
        /// </summary>
        [CanBeNull] public readonly object source;

        /// <summary>
        ///     Target entity
        /// </summary>
        [NotNull] public readonly EntityBase target;
        
        /// <summary>
        ///     Damage affinity
        /// </summary>
        [CanBeNull] public readonly AffinityType affinityType;

        /// <summary>
        ///     Resistance value
        /// </summary>
        public readonly float resistanceValue;

        /// <summary>
        ///     Amount of damage
        /// </summary>
        public readonly int amount;
        
        public DamageContext UpdateAmount(int healthToChange)
        {
            return new DamageContext(target, source, affinityType, resistanceValue, healthToChange);
        }
        
        public DamageContext(
            [NotNull] EntityBase target,
            [CanBeNull] object source,
            [CanBeNull] AffinityType affinityType,
            float resistanceValue,
            int amount)
        {
            Assert.IsTrue(amount >= 0, "Amount of damage must be greater than or equal to zero");
            Assert.IsNotNull(target, "Target cannot be null");
            this.target = target;
            this.source = source;
            this.affinityType = affinityType;
            this.resistanceValue = resistanceValue;
            this.amount = (int) (amount * math.clamp(1 - resistanceValue, 0, 1));
        }

        public static DamageContext Create<TDamageAffinity>(
            [NotNull] EntityBase target,
            [CanBeNull] object source,
            int amount)
            where TDamageAffinity : AffinityType, new()
        {
            Assert.IsNotNull(target, "Target cannot be null");
            Assert.IsTrue(amount >= 0, "Amount of damage must be greater than or equal to zero");
            
            float resistanceValue = target.GetResistance<TDamageAffinity>();
            return new DamageContext(target, source, AffinityDatabase.GetExact<TDamageAffinity>(), resistanceValue, amount);
        }

     
    }
}