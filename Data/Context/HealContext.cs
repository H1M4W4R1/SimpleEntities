using JetBrains.Annotations;
using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Data.Affinity;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Systems.SimpleEntities.Data.Context
{
    /// <summary>
    ///     Context for healing
    /// </summary>
    public readonly ref struct HealContext
    {
        /// <summary>
        ///     Source of the healing
        /// </summary>
        [CanBeNull] public readonly object source;

        /// <summary>
        ///     Target of the healing
        /// </summary>
        [NotNull] public readonly AliveEntityBase target;

        /// <summary>
        ///     Healing affinity
        /// </summary>
        [CanBeNull] public readonly AffinityType affinityType;

        /// <summary>
        ///     Resistance value
        /// </summary>
        public readonly float resistanceValue;

        /// <summary>
        ///     Amount of healing
        /// </summary>
        public readonly long amount;
        
        public HealContext(
            [NotNull] AliveEntityBase target,
            [CanBeNull] object source,
            [CanBeNull] AffinityType affinityType,
            float resistanceValue,
            long amount)
        {
            Assert.IsTrue(amount >= 0, "Amount of healing must be greater than or equal to zero");
            Assert.IsNotNull(target, "Target cannot be null");
            this.target = target;
            this.source = source;
            this.affinityType = affinityType;
            this.resistanceValue = resistanceValue;
            this.amount = (long) (amount * math.clamp(1 - resistanceValue, 0, 1));
        }

        public static HealContext Create<TDamageAffinity>(
            [NotNull] AliveEntityBase target,
            [CanBeNull] object source,
            long amount)
            where TDamageAffinity : AffinityType, new()
        {
            Assert.IsNotNull(target, "Target cannot be null");
            Assert.IsTrue(amount >= 0, "Amount of healing must be greater than or equal to zero");

            float resistanceValue = target.GetResistance<TDamageAffinity>();
            return new HealContext(target, source, AffinityDatabase.GetExact<TDamageAffinity>(), resistanceValue,
                amount);
        }
    }
}