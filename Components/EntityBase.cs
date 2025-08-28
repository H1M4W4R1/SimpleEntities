using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleEntities.Data;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data.Collections;
using Systems.SimpleStats.Data.Statistics;
using UnityEngine;

namespace Systems.SimpleEntities.Components
{
    /// <summary>
    ///     Represents in-game entity that may have stats, inventory, etc.
    ///     Generally used to represent player, enemy, etc., but not objects such as chest.
    /// </summary>
    /// <remarks>
    ///     Intended to be used for objects that have health, statistics etc.
    /// </remarks>
    public abstract class EntityBase : MonoBehaviour, IWithStatModifiers
    {
#region Unity Lifecycle

        protected void Awake()
        {
            // Refresh entity modifiers for first time
            RefreshModifiersIfNecessary();
            
            // Reset health to max
            ResetHealthToMax();
        }
        
        /// <summary>
        ///     Resets health to max
        /// </summary>
        /// <remarks>
        ///     Can be overriden to perform additional computation e.g. using statistic modifiers.
        ///     Does not trigger heal events, should be used only for initial reset.
        /// </remarks>
        protected virtual void ResetHealthToMax() => CurrentHealth = MaxHealth;

#endregion

#region Damage and healing

        /// <summary>
        ///     Current health of the entity
        /// </summary>
        [field: SerializeField] public int CurrentHealth { get; protected set; }

        /// <summary>
        ///     Maximum health of the entity
        /// </summary>
        [field: SerializeField] public int MaxHealth { get; protected set; }

        /// <summary>
        ///     Heals the entity
        /// </summary>
        /// <param name="context">Context of the healing event</param>
        public void Heal(in HealContext context)
        {
            // Check if entity can be healed
            if (!CanBeHealed(context)) return;

            // Add health and execute heal handlers
            CurrentHealth += context.amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            OnHealReceived(context);
        }

        /// <summary>
        ///     Deals damage to the entity
        /// </summary>
        /// <param name="context">Context of the damage event</param>
        public void Damage(in DamageContext context)
        {
            // Check if entity can be damaged
            if (!CanBeDamaged(context)) return;

            // Subtract health and execute damage handlers
            CurrentHealth -= context.amount;
            OnDamageReceived(context);

            // If health is zero or less, kill the entity
            if (CurrentHealth <= 0) Kill(context);
        }

        /// <summary>
        ///     Kills the entity
        /// </summary>
        public void Kill(in DamageContext context)
        {
            // Reset health to zero (just in case)
            CurrentHealth = 0;

            // Check death save
            DeathSaveContext deathSaveContext = CheckDeathSave(context);

            // If entity should be saved, set health to the value specified in death save context
            if (deathSaveContext.shouldBeSaved)
            {
                CurrentHealth = deathSaveContext.healthToSet;
                return;
            }

            // Perform death events
            OnDeath(context);
        }

        /// <summary>
        ///     Executes when entity takes damage
        /// </summary>
        /// <param name="context">Context of the damage event</param>
        protected virtual void OnDamageReceived(in DamageContext context)
        {
        }

        /// <summary>
        ///     Checks if entity can be damaged
        /// </summary>
        /// <param name="context">Context of the damage event</param>
        /// <returns>True if entity can be damaged</returns>
        protected virtual bool CanBeDamaged(in DamageContext context) => true;

        /// <summary>
        ///     Executes when entity takes healing
        /// </summary>
        /// <param name="context">Context of the healing event</param>
        protected virtual void OnHealReceived(in HealContext context)
        {
        }

        /// <summary>
        ///     Checks if entity can be healed
        /// </summary>
        /// <param name="context">Context of the heal event</param>
        /// <returns>True if entity can be healed</returns>
        protected virtual bool CanBeHealed(in HealContext context) => true;

        /// <summary>
        ///     Checks if entity should be protected from death
        /// </summary>
        /// <param name="context">Context of the damage event</param>
        /// <returns>True if entity should not be killed</returns>
        protected virtual DeathSaveContext CheckDeathSave(in DamageContext context) => default;

        /// <summary>
        ///     Executes when entity dies
        /// </summary>
        /// <param name="context">Context of the damage event that killed the entity</param>
        protected virtual void OnDeath(in DamageContext context)
        {
        }

#endregion

#region Statistics and modifiers

        /// <summary>
        ///     Modifiers registered for this object
        /// </summary>
        protected readonly List<IStatModifier> statModifiers = new();

        /// <summary>
        ///     Gets all modifiers registered for this object
        /// </summary>
        /// <returns>Read-only list of modifiers</returns>
        public IReadOnlyList<IStatModifier> GetAllModifiers() => statModifiers;

        /// <summary>
        ///     Gets modifiers for statistic
        /// </summary>
        /// <typeparam name="TStatisticType">Type of statistic</typeparam>
        /// <returns>Enumerable of modifiers for statistic</returns>
        [ItemNotNull] public IEnumerable<IStatModifier> GetModifiersFor<TStatisticType>()
            where TStatisticType : StatisticBase
        {
            // Loop through modifiers and yield return only those that are of type TStatisticType
            for (int index = 0; index < statModifiers.Count; index++)
            {
                IStatModifier modifier = statModifiers[index];
                if (modifier is IStatModifier<TStatisticType>) yield return modifier;
            }
        }

        /// <summary>
        ///     Updates modifiers collection
        /// </summary>
        public void TransferModifiersTo<TStatisticType>(StatModifierCollection statModifierCollection)
        {
            RefreshModifiersIfNecessary();
            statModifierCollection.AddRange(statModifiers);
        }

        /// <summary>
        ///     Refresh statistic modifiers if necessary
        /// </summary>
        /// <remarks>
        ///     Intended to refresh modifiers when e.g. equipment changes.
        ///     Entity should cache all modifiers related to statistics it has including things
        ///     such as equipment, etc.
        /// </remarks>
        public virtual void RefreshModifiersIfNecessary()
        {
            // Do nothing by default
        }

#endregion
    }
}