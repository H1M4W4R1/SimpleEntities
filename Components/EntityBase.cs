using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleEntities.Affinity;
using Systems.SimpleEntities.Data.Context;
using Systems.SimpleEntities.Resistances;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Collections;
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
        ///     Gets resistance of the entity
        /// </summary>
        /// <typeparam name="TAffinityType">Type of the affinity</typeparam>
        /// <returns>Value of the resistance</returns>
        public float GetResistance<TAffinityType>()
            where TAffinityType : DamageAffinity
        {
            float result = 0;
            
            // Get all resistances from database
            IReadOnlyList<ResistanceBase> resistances = StatsDatabase.GetAll<ResistanceBase>();

            IWithStatModifiers statsAccess = this;
            foreach (ResistanceBase resistance in resistances)
            {
                if (!resistance.IsValidFor<TAffinityType>()) continue;
                
                StatModifierCollection modifierCollection = new();
                modifierCollection.AddRange(statsAccess.GetAllModifiersFor(resistance));

                float finalValue = resistance.GetFinalValue(modifierCollection);
                result += finalValue;
            }
            
            return result;
        }

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
            
            if(!ReferenceEquals(context.healingAffinity, null))
                context.healingAffinity.OnHealingReceived(context);
        }

        /// <summary>
        ///     Deals damage to the entity
        /// </summary>
        /// <param name="source">Source of the damage</param>
        /// <param name="amount">Base amount of damage</param>
        /// <typeparam name="TDamageAffinity">Affinity of the damage</typeparam>
        public void Damage<TDamageAffinity>(
            [CanBeNull] object source,
            int amount)
            where TDamageAffinity : DamageAffinity
        {
            DamageContext context = DamageContext.Create<TDamageAffinity>(this, source, amount);
            Damage(context);
        }
        
        /// <summary>
        ///     Heals the entity
        /// </summary>
        /// <param name="source">Source of the healing</param>
        /// <param name="amount">Base amount of healing</param>
        /// <typeparam name="THealingAffinity">Affinity of the healing</typeparam>
        public void Heal<THealingAffinity>(
            [CanBeNull] object source,
            int amount)
            where THealingAffinity : DamageAffinity
        {
            HealContext context = HealContext.Create<THealingAffinity>(this, source, amount);
            Heal(context);
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
            
            if(!ReferenceEquals(context.damageAffinity, null))
                context.damageAffinity.OnDamageReceived(context);

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
            
            if(!ReferenceEquals(context.damageAffinity, null))
                context.damageAffinity.OnDeath(context);
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