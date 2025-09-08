using Systems.SimpleCore.Automation.Attributes;
using Systems.SimpleCore.Operations;
using Systems.SimpleEntities.Data.Context;
using Systems.SimpleEntities.Operations;
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
        protected internal virtual OperationResult CanBeDamaged(in DamageContext context)
            => EntityOperations.Permitted();

        /// <summary>
        ///     Checks if entity can be healed
        /// </summary>
        protected internal virtual OperationResult CanBeHealed(in HealContext context)
            => EntityOperations.Permitted();

        /// <summary>
        ///     Checks if entity can be saved from death and heals entity to desired health amount
        /// </summary>
        protected internal virtual DeathSaveContext CanSaveFromDeath(in DamageContext context) =>
            new(false, 0);

#endregion

        /// <summary>
        ///     Executed when entity takes damage
        /// </summary>
        protected internal virtual void OnDamageReceived(
            in DamageContext context,
            in OperationResult result,
            long healthLost)
        {
        }

        /// <summary>
        ///     Executed when damage is failed due to <see cref="CanBeDamaged"/>
        /// </summary>
        protected internal virtual void OnDamageFailed(
            in DamageContext context,
            in OperationResult result)
        {
        }


        /// <summary>
        ///     Executed when entity dies
        /// </summary>
        protected internal virtual void OnDeath(
            in DamageContext context,
            in OperationResult result,
            long healthLost)
        {
        }


        /// <summary>
        ///     Executed when entity takes healing
        /// </summary>
        protected internal virtual void OnHealingReceived(
            in HealContext context,
            in OperationResult result,
            long healthAdded)
        {
        }

        /// <summary>
        ///     Executed when healing is failed due to <see cref="CanBeHealed"/>
        /// </summary>
        protected internal virtual void OnHealingFailed(
            in HealContext context,
            in OperationResult result)
        {
        }

        protected internal virtual void OnSavedFromDeath(
            in DamageContext damageContext,
            in DeathSaveContext context,
            in OperationResult result,
            long healthSet)
        {
        }
    }
}