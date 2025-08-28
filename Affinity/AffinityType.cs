using Systems.SimpleEntities.Data.Context;
using UnityEngine;

namespace Systems.SimpleEntities.Affinity
{
    /// <summary>
    ///     Type of damage, used to determine affinity
    /// </summary>
    public abstract class AffinityType : ScriptableObject
    {
        /// <summary>
        ///     Executed when entity takes damage
        /// </summary>
        protected internal virtual void OnDamageReceived(in DamageContext context) { }
        
        /// <summary>
        ///     Executed when entity dies
        /// </summary>
        protected internal virtual void OnDeath(in DamageContext context) { }
        
        /// <summary>
        ///     Executed when entity takes healing
        /// </summary>
        protected internal virtual void OnHealingReceived(in HealContext context) { }
        
    }
}