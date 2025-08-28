using JetBrains.Annotations;
using Systems.SimpleEntities.Affinity;
using Systems.SimpleEntities.Components;
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
        [NotNull] public readonly EntityBase target;

        /// <summary>
        ///     Healing affinity
        /// </summary>
        [CanBeNull] public readonly AffinityType healingAffinityType;

        /// <summary>
        ///     Resistance value
        /// </summary>
        public readonly float resistanceValue;

        /// <summary>
        ///     Amount of healing
        /// </summary>
        public readonly int amount;

        public HealContext(
            [NotNull] EntityBase target,
            [CanBeNull] object source,
            [CanBeNull] AffinityType healingAffinityType,
            float resistanceValue,
            int amount)
        {
            Assert.IsTrue(amount >= 0, "Amount of healing must be greater than or equal to zero");
            Assert.IsNotNull(target, "Target cannot be null");
            this.target = target;
            this.source = source;
            this.healingAffinityType = healingAffinityType;
            this.resistanceValue = resistanceValue;
            this.amount = (int) (amount * math.clamp(1 - resistanceValue, 0, 1));
        }

        public static HealContext Create<TDamageAffinity>(
            [NotNull] EntityBase target,
            [CanBeNull] object source,
            int amount)
            where TDamageAffinity : AffinityType
        {
            Assert.IsNotNull(target, "Target cannot be null");
            Assert.IsTrue(amount >= 0, "Amount of healing must be greater than or equal to zero");

            float resistanceValue = target.GetResistance<TDamageAffinity>();
            return new HealContext(target, source, AffinityDatabase.Get<TDamageAffinity>(), resistanceValue,
                amount);
        }
    }
}