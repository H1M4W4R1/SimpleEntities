using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.Serialization;
using Systems.SimpleEntities.Data;
using Systems.SimpleEntities.Data.Affinity;
using Systems.SimpleEntities.Data.Context;
using Systems.SimpleEntities.Data.Resistances;
using Systems.SimpleEntities.Data.Status.Abstract;
using Systems.SimpleEntities.Data.Status.Enums;
using Systems.SimpleEntities.Data.Status.Storage;
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
#region Save and Load

        // TODO: Better solution for save/load system?
        public byte[] Save() =>
            SerializationUtility.SerializeValue(appliedStatuses, DataFormat.Binary);

        public void Load(byte[] data)
        {
            List<AppliedStatusData> parsedData =
                SerializationUtility.DeserializeValue<List<AppliedStatusData>>(data, DataFormat.Binary);
            appliedStatuses.Clear();
            appliedStatuses.AddRange(parsedData);
        }

#endregion

#region Unity Lifecycle

        protected void Awake()
        {
            // Refresh entity modifiers for first time
            RefreshModifiersIfNecessary();

            // Reset health to max
            ResetHealthToMax();
        }

        protected void Update()
        {
            // Handle statuses
            HandleStatusTick(Time.deltaTime);
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
            where TAffinityType : AffinityType
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

            if (!ReferenceEquals(context.healingAffinityType, null))
                context.healingAffinityType.OnHealingReceived(context);
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
            where TDamageAffinity : AffinityType, new()
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
            where THealingAffinity : AffinityType, new()
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

            if (!ReferenceEquals(context.affinityType, null)) context.affinityType.OnDamageReceived(context);

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

            if (!ReferenceEquals(context.affinityType, null)) context.affinityType.OnDeath(context);
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

#region Status effects

        /// <summary>
        ///     Container for all applied status effects
        /// </summary>
        protected readonly List<AppliedStatusData> appliedStatuses = new();

        /// <summary>
        ///     Acquires all applied status effects
        /// </summary>
        /// <returns>Read-only list of applied status effects</returns>
        public IReadOnlyList<AppliedStatusData> GetAllAppliedStatuses() => appliedStatuses;

        /// <summary>
        ///     Applies a status to the entity
        /// </summary>
        /// <param name="stackCount">Stack count to apply</param>
        /// <typeparam name="TStatusType">Type of the status to apply</typeparam>
        /// <returns>Result of the application</returns>
        public ApplyStatusResult ApplyStatus<TStatusType>(int stackCount = 1)
            where TStatusType : StatusBase, new()
        {
            TStatusType status = StatusDatabase.GetExact<TStatusType>();
            if (ReferenceEquals(status, null)) return ApplyStatusResult.InvalidStatus;
            return ApplyStatus(status, stackCount);
        }

        /// <summary>
        ///     Applies a status to the entity
        /// </summary>
        /// <param name="status">Status to apply</param>
        /// <param name="stackCount">Stack count to apply</param>
        /// <returns>Result of the application</returns>
        public ApplyStatusResult ApplyStatus([NotNull] StatusBase status, int stackCount = 1)
        {
            // Find status if already applied
            AppliedStatusData statusReference = default;
            int statusReferenceIndex = -1;
            for (int i = 0; i < appliedStatuses.Count; i++)
            {
                if (appliedStatuses[i].status != status) continue;
                statusReference = appliedStatuses[i];
                statusReferenceIndex = i;
                break;
            }

            // If status is not applied, apply it
            if (ReferenceEquals(statusReference.status, null))
            {
                StatusContext addStatusContext = new(this, status, stackCount);
                statusReference = new AppliedStatusData(status, stackCount);
                appliedStatuses.Add(statusReference);
                statusReference.status.OnStatusApplied(addStatusContext);
                return ApplyStatusResult.Success;
            }

            // If status is already applied, check if it can be stacked (or if max stack is reached)
            if (statusReference.stackCount + stackCount > status.MaxStack && status.MaxStack > 0)
                return ApplyStatusResult.MaxStackReached;

            // If status can be stacked, stack it
            StatusContext modifyStatusContext = new(this, status, stackCount);
            statusReference.stackCount += stackCount;
            statusReference.status.OnStatusStackChanged(modifyStatusContext);
            appliedStatuses[statusReferenceIndex] = statusReference;
            return ApplyStatusResult.Success;
        }

        /// <summary>
        ///     Removes a status from the entity
        /// </summary>
        /// <param name="stackCount">Stack count to remove</param>
        /// <param name="force">If true, will remove status even if it has not enough stacks</param>
        /// <typeparam name="TStatusType">Type of the status to remove</typeparam>
        /// <returns>Result of the removal</returns>
        public RemoveStatusResult RemoveStatus<TStatusType>(int stackCount = 1, bool force = true)
            where TStatusType : StatusBase, new()
        {
            TStatusType status = StatusDatabase.GetExact<TStatusType>();
            if (ReferenceEquals(status, null)) return RemoveStatusResult.InvalidStatus;
            return RemoveStatus(status);
        }

        /// <summary>
        ///     Removes a status from the entity
        /// </summary>
        /// <param name="status">Status to remove</param>
        /// <param name="stackCount">Stack count to remove</param>
        /// <param name="force">If true, will remove status even if it has not enough stacks</param>
        /// <returns>Result of the removal</returns>
        public RemoveStatusResult RemoveStatus([NotNull] StatusBase status, int stackCount = 1, bool force = true)
        {
            // Find status if already applied
            AppliedStatusData statusReference = default;
            int statusReferenceIndex = -1;
            for (int i = 0; i < appliedStatuses.Count; i++)
            {
                if (appliedStatuses[i].status != status) continue;

                statusReference = appliedStatuses[i];
                statusReferenceIndex = i;
                break;
            }

            // If status is not applied, return invalid status
            if (ReferenceEquals(statusReference.status, null)) return RemoveStatusResult.NotApplied;

            // If status is applied, check if it can be removed
            if (statusReference.stackCount - stackCount < 0 && !force) return RemoveStatusResult.NotEnoughStacks;

            // Remove stacks
            statusReference.stackCount -= stackCount;

            // If status is now empty, remove it from the list
            if (statusReference.stackCount == 0 && statusReferenceIndex != -1)
            {
                StatusContext removeStatusContext = new(this, status, statusReference.stackCount);
                statusReference.status.OnStatusRemoved(removeStatusContext);

                // Remove status from list
                appliedStatuses.RemoveAt(statusReferenceIndex);
            }
            else // If not, then handle stack reduction
            {
                StatusContext reduceStackContext = new(this, status, -stackCount);
                statusReference.status.OnStatusStackChanged(reduceStackContext);

                // Update applied statuses
                appliedStatuses[statusReferenceIndex] = statusReference;
            }

            return RemoveStatusResult.Success;
        }

        /// <summary>
        ///     Checks if the entity has a status
        /// </summary>
        /// <typeparam name="TStatusType">Type of the status to check for</typeparam>
        /// <returns>True if the entity has the status, false otherwise</returns>
        public bool HasStatus<TStatusType>()
            where TStatusType : StatusBase, new()
        {
            TStatusType status = StatusDatabase.GetExact<TStatusType>();
            if (ReferenceEquals(status, null)) return false;
            return HasStatus(status);
        }

        /// <summary>
        ///     Checks if the entity has a status
        /// </summary>
        /// <param name="status">Status to check for</param>
        /// <returns>True if the entity has the status, false otherwise</returns>
        public bool HasStatus([NotNull] StatusBase status) =>
            GetStatusStackCount(status) > 0;

        /// <summary>
        ///     Gets the stack count of a status
        /// </summary>
        /// <typeparam name="TStatusType">Type of the status to get the stack count for</typeparam>
        /// <returns>Stack count of the status</returns>
        public int GetStatusStackCount<TStatusType>()
            where TStatusType : StatusBase, new()
        {
            TStatusType status = StatusDatabase.GetExact<TStatusType>();
            if (ReferenceEquals(status, null)) return 0;
            return GetStatusStackCount(status);
        }

        /// <summary>
        ///     Gets the stack count of a status
        /// </summary>
        /// <param name="status">Status to check for</param>
        /// <returns>Stack count of the status</returns>
        public int GetStatusStackCount([NotNull] StatusBase status)
        {
            for (int i = 0; i < appliedStatuses.Count; i++)
            {
                if (appliedStatuses[i].status != status) continue;

                return appliedStatuses[i].stackCount;
            }

            return 0;
        }

        /// <summary>
        ///     Performs per-tick status handling
        /// </summary>
        protected void HandleStatusTick(float deltaTime)
        {
            for (int i = 0; i < appliedStatuses.Count; i++)
            {
                StatusContext tickContext = new(this, appliedStatuses[i].status, appliedStatuses[i].stackCount);
                appliedStatuses[i].status.OnStatusTick(tickContext, deltaTime);
            }
        }

#endregion
    }
}