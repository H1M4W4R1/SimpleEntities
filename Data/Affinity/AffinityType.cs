using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleEntities.Data.Context;
using UnityEngine;

namespace Systems.SimpleEntities.Data.Affinity
{
    /// <summary>
    ///     Type of damage, used to determine affinity
    /// </summary>
    [AutoCreate("Affinities", AffinityDatabase.LABEL)]
    public abstract class AffinityType : ScriptableObject
    {
        protected internal virtual bool CanBeDamaged(in DamageContext context) => true;
        
        /// <summary>
        ///     Executed when entity takes damage
        /// </summary>
        protected internal virtual void OnDamageReceived(in DamageContext context) { }
        
        protected internal virtual void OnDamageFailed(in DamageContext context) { }

        protected internal virtual DeathSaveContext CanSaveFromDeath(in DamageContext context) =>
            new(false, 0);
        
        /// <summary>
        ///     Executed when entity dies
        /// </summary>
        protected internal virtual void OnDeath(in DamageContext context) { }
        
        protected internal virtual bool CanBeHealed(in HealContext context) => true;
        
        /// <summary>
        ///     Executed when entity takes healing
        /// </summary>
        protected internal virtual void OnHealingReceived(in HealContext context) { }
        
        protected internal virtual void OnHealingFailed(in HealContext context) { }
        
    }
}