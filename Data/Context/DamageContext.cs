using JetBrains.Annotations;
using Systems.SimpleEntities.Affinity;
using Systems.SimpleEntities.Components;
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
        [CanBeNull] public readonly DamageAffinity damageAffinity;

        /// <summary>
        ///     Resistance value
        /// </summary>
        public readonly float resistanceValue;

        /// <summary>
        ///     Amount of damage
        /// </summary>
        public readonly int amount;
        
        public DamageContext(
            [NotNull] EntityBase target,
            [CanBeNull] object source,
            [CanBeNull] DamageAffinity damageAffinity,
            float resistanceValue,
            int amount)
        {
            Assert.IsTrue(amount >= 0, "Amount of damage must be greater than or equal to zero");
            Assert.IsNotNull(target, "Target cannot be null");
            this.target = target;
            this.source = source;
            this.damageAffinity = damageAffinity;
            this.resistanceValue = resistanceValue;
            this.amount = (int) (amount * math.clamp(1 - resistanceValue, 0, 1));
        }

        public static DamageContext Create<TDamageAffinity>(
            [NotNull] EntityBase target,
            [CanBeNull] object source,
            int amount)
            where TDamageAffinity : DamageAffinity
        {
            Assert.IsNotNull(target, "Target cannot be null");
            Assert.IsTrue(amount >= 0, "Amount of damage must be greater than or equal to zero");
            
            float resistanceValue = target.GetResistance<TDamageAffinity>();
            return new DamageContext(target, source, AffinityDatabase.GetAffinity<TDamageAffinity>(), resistanceValue, amount);
        }
    }
}