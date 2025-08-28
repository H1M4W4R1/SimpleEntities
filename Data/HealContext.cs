using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Systems.SimpleEntities.Data
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
        ///     Amount of healing
        /// </summary>
        public readonly int amount;

        public HealContext([CanBeNull] object source, int amount)
        {
            Assert.IsTrue(amount >= 0, "Amount of healing must be greater than or equal to zero");
            this.source = source;
            this.amount = amount;
        }
    }
}