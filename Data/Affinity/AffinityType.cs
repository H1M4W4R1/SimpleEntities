using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleEntities.Data.Context;
using UnityEngine;

namespace Systems.SimpleEntities.Data.Affinity
{
    /// <summary>
    ///     Type of damage, used to determine affinity
    /// </summary>
    [AutoCreate("Affinities", AffinityDatabase.LABEL)] public abstract class AffinityType : ScriptableObject
    {
#region Checks

        /// <summary>
        ///     Checks if entity can be damaged
        /// </summary>
        public virtual bool CanBeDamaged(in DamageContext context) => true;
        
        /// <summary>
        ///     Checks if entity can be healed
        /// </summary>
        public virtual bool CanBeHealed(in HealContext context) => true;

        /// <summary>
        ///     Checks if entity can be saved from death and heals entity to desired health amount
        /// </summary>
        public virtual DeathSaveContext CanSaveFromDeath(in DamageContext context) =>
            new(false, 0);

#endregion

        /// <summary>
        ///     Executed when entity takes damage
        /// </summary>
        protected internal virtual void OnDamageReceived(in DamageContext context)
        {
        }
        
        /// <summary>
        ///     Executed when damage is failed due to <see cref="CanBeDamaged"/>
        /// </summary>
        protected internal virtual void OnDamageFailed(in DamageContext context)
        {
        }


        /// <summary>
        ///     Executed when entity dies
        /// </summary>
        protected internal virtual void OnDeath(in DamageContext context)
        {
        }


        /// <summary>
        ///     Executed when entity takes healing
        /// </summary>
        protected internal virtual void OnHealingReceived(in HealContext context)
        {
        }

        /// <summary>
        ///     Executed when healing is failed due to <see cref="CanBeHealed"/>
        /// </summary>
        protected internal virtual void OnHealingFailed(in HealContext context)
        {
        }
    }
}