using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace Systems.SimpleEntities.Data
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
        ///     Amount of damage
        /// </summary>
        public readonly int amount;

        public DamageContext(
            [CanBeNull] object source,
            int amount)
        {
            Assert.IsTrue(amount >= 0, "Amount of damage must be greater than or equal to zero");
            this.source = source;
            this.amount = amount;
        }
    }
}