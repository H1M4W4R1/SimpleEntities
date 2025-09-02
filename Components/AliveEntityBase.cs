using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleCore.Operations;
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleEntities.Data;
using Systems.SimpleEntities.Data.Affinity;
using Systems.SimpleEntities.Data.Context;
using Systems.SimpleEntities.Data.Enums;
using Systems.SimpleEntities.Data.Resistances;
using Systems.SimpleEntities.Data.Status.Abstract;
using Systems.SimpleEntities.Data.Status.Enums;
using Systems.SimpleEntities.Data.Status.Storage;
using Systems.SimpleEntities.Operations;
using Systems.SimpleStats.Abstract;
using Systems.SimpleStats.Abstract.Modifiers;
using Systems.SimpleStats.Data;
using Systems.SimpleStats.Data.Collections;
using Unity.Mathematics;
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
    public abstract class AliveEntityBase : TickingEntityBase, IWithStatModifiers
    {
#region Entity Lifecycle

        protected override void AssignComponents()
        {
            // Refresh entity modifiers for first time
            RefreshModifiersIfNecessary();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Reset health to max
            ResetHealthToMax();
        }

        protected override void OnTick(float deltaTime)
        {
            // Handle statuses
            HandleStatusTick(deltaTime);
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
        [field: SerializeField] public virtual long CurrentHealth { get; protected set; }

        /// <summary>
        ///     Maximum health of the entity
        /// </summary>
        [field: SerializeField] public virtual long MaxHealth { get; protected set; }

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
        /// <param name="actionSource">Source of the action</param>
        /// <returns>Result of the operation with amount of healed health</returns>
        public OperationResult<long> Heal(
            in HealContext context,
            ActionSource actionSource = ActionSource.External)
        {
            // Check if entity can be healed
            OperationResult canHealResult = CanBeHealed(context);
            if (!canHealResult)
            {
                if (actionSource == ActionSource.Internal) return canHealResult.WithData(0L);
                OnHealFailed(context, canHealResult.WithData(context.amount));
                return canHealResult.WithData(0L);
            }

            // Compute amount of health to change
            // and update context with final amount
            long missingHealth = MaxHealth - CurrentHealth;
            long healthToChange = math.min(missingHealth, context.amount);

            // Add health and execute heal handlers
            CurrentHealth += healthToChange;

            OperationResult<long> opResult = EntityOperations.Healed().WithData(healthToChange);
            if (actionSource == ActionSource.Internal) return opResult;
            OnHealReceived(context, opResult);
            return opResult;
        }

        /// <summary>
        ///     Deals damage to the entity
        /// </summary>
        /// <param name="source">Source of the damage</param>
        /// <param name="amount">Base amount of damage</param>
        /// <param name="actionSource">Source of the action</param>
        /// <typeparam name="TDamageAffinity">Affinity of the damage</typeparam>
        public OperationResult<long> Damage<TDamageAffinity>(
            [CanBeNull] object source,
            long amount,
            ActionSource actionSource = ActionSource.External)
            where TDamageAffinity : AffinityType, new()
        {
            DamageContext context = DamageContext.Create<TDamageAffinity>(this, source, amount);
            return Damage(context, actionSource);
        }

        /// <summary>
        ///     Heals the entity
        /// </summary>
        /// <param name="source">Source of the healing</param>
        /// <param name="amount">Base amount of healing</param>
        /// <param name="actionSource">Source of the action</param>
        /// <typeparam name="THealingAffinity">Affinity of the healing</typeparam>
        public OperationResult<long> Heal<THealingAffinity>(
            [CanBeNull] object source,
            long amount,
            ActionSource actionSource = ActionSource.External)
            where THealingAffinity : AffinityType, new()
        {
            HealContext context = HealContext.Create<THealingAffinity>(this, source, amount);
            return Heal(context, actionSource);
        }

        /// <summary>
        ///     Deals damage to the entity
        /// </summary>
        /// <param name="context">Context of the damage event</param>
        /// <param name="actionSource">Source of the action</param>
        public OperationResult<long> Damage(
            in DamageContext context,
            ActionSource actionSource = ActionSource.External)
        {
            // Check if entity can be damaged
            OperationResult canBeDamagedResult = CanBeDamaged(context);
            if (!CanBeDamaged(context))
            {
                if (actionSource == ActionSource.Internal) return canBeDamagedResult.WithData(0L);
                OnDamageFailed(context, canBeDamagedResult.WithData(context.amount));
                return canBeDamagedResult.WithData(0L);
            }

            // Compute amount of health to change
            long healthToChange = math.min(context.amount, CurrentHealth);

            // Subtract health and execute damage handlers
            CurrentHealth -= healthToChange;

            OperationResult<long> opResult = EntityOperations.Damaged().WithData(healthToChange);
            if (actionSource == ActionSource.External)
            {
                OnDamageReceived(context, opResult);
            }

            // If health is zero or less, kill the entity
            if (CurrentHealth > 0) return opResult;
            return Kill(context, healthBeforeDeath: healthToChange);
        }

        /// <summary>
        ///     Kills the entity
        /// </summary>
        /// <returns>Result of the operation with amount of health after "death"</returns>
        public OperationResult<long> Kill(
            in DamageContext context,
            ActionSource actionSource = ActionSource.External,
            long healthBeforeDeath = -1)
        {
            // Copy health if not specified
            if (healthBeforeDeath < 0) healthBeforeDeath = CurrentHealth;

            // Reset health to zero (just in case)
            CurrentHealth = 0;

            // Check death save
            DeathSaveContext deathSaveContext = CanSaveFromDeath(context);

            // If entity should be saved, set health to the value specified in death save context
            if (deathSaveContext.shouldBeSaved)
            {
                CurrentHealth = deathSaveContext.healthToSet;

                OperationResult<long> deathSaveData =
                    EntityOperations.SavedFromDeath().WithData(CurrentHealth);
                if (actionSource == ActionSource.Internal) return deathSaveData;

                OnSavedFromDeath(context, deathSaveContext, deathSaveData);
                return deathSaveData;
            }

            // Perform death events
            if (actionSource == ActionSource.Internal) return EntityOperations.Killed().WithData(0L);
            OnDeath(context, EntityOperations.Killed().WithData(healthBeforeDeath));
            return EntityOperations.Killed().WithData(0L);
        }


        /// <summary>
        ///     Checks if entity can be damaged
        /// </summary>
        public virtual OperationResult CanBeDamaged(in DamageContext context)
        {
            if (!ReferenceEquals(context.affinityType, null)) return context.affinityType.CanBeDamaged(context);
            return EntityOperations.Permitted();
        }

        /// <summary>
        ///     Checks if entity can be healed
        /// </summary>
        public virtual OperationResult CanBeHealed(in HealContext context)
        {
            if (!ReferenceEquals(context.affinityType, null)) return context.affinityType.CanBeHealed(context);
            return EntityOperations.Permitted();
        }

        /// <summary>
        ///     Called when damage is failed due to <see cref="CanBeDamaged"/>
        /// </summary>
        protected virtual void OnDamageFailed(in DamageContext context, in OperationResult<long> healthToTake)
        {
            if (!ReferenceEquals(context.affinityType, null))
                context.affinityType.OnDamageFailed(context, healthToTake);
        }

        /// <summary>
        ///     Executes when entity takes damage
        /// </summary>
        protected virtual void OnDamageReceived(in DamageContext context, in OperationResult<long> healthLost)
        {
            if (!ReferenceEquals(context.affinityType, null))
                context.affinityType.OnDamageReceived(context, healthLost);
        }


        /// <summary>
        ///     Called when healing is failed due to <see cref="CanBeHealed"/>
        /// </summary>
        protected virtual void OnHealFailed(in HealContext context, in OperationResult<long> healthToAdd)
        {
            if (!ReferenceEquals(context.affinityType, null))
                context.affinityType.OnHealingFailed(context, healthToAdd);
        }

        /// <summary>
        ///     Executes when entity takes healing
        /// </summary>
        protected virtual void OnHealReceived(in HealContext context, in OperationResult<long> healthAdded)
        {
            if (!ReferenceEquals(context.affinityType, null))
                context.affinityType.OnHealingReceived(context, healthAdded);
        }

        /// <summary>
        ///     Checks if entity should be protected from death
        /// </summary>
        protected virtual DeathSaveContext CanSaveFromDeath(in DamageContext context)
        {
            if (!ReferenceEquals(context.affinityType, null))
                return context.affinityType.CanSaveFromDeath(context);
            return new DeathSaveContext(false, 0);
        }

        /// <summary>
        ///     Called when entity is saved from death
        /// </summary>
        protected virtual void OnSavedFromDeath(
            in DamageContext damageContext,
            in DeathSaveContext context,
            in OperationResult<long> healthSet)
        {
            if (!ReferenceEquals(damageContext.affinityType, null))
                damageContext.affinityType.OnSavedFromDeath(damageContext, context, healthSet);
        }

        /// <summary>
        ///     Executes when entity dies
        /// </summary>
        protected virtual void OnDeath(in DamageContext context, in OperationResult<long> healthLost)
        {
            if (!ReferenceEquals(context.affinityType, null)) context.affinityType.OnDeath(context, healthLost);
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
        private readonly List<AppliedStatusData> _appliedStatuses = new();

        /// <summary>
        ///     Acquires all applied status effects
        /// </summary>
        /// <returns>Read-only list of applied status effects</returns>
        public IReadOnlyList<AppliedStatusData> GetAllAppliedStatuses() => _appliedStatuses;

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
        /// <param name="flags">Flags to modify the application</param>
        /// <returns>Result of the application</returns>
        public ApplyStatusResult ApplyStatus(
            [NotNull] StatusBase status,
            int stackCount = 1,
            StatusModificationFlags flags = StatusModificationFlags.None)
        {
            // Create status context
            StatusContext checkContext = new(this, status, stackCount);

            // Check if status can be applied to the entity
            if (!CanApplyStatus(checkContext) && (flags & StatusModificationFlags.IgnoreConditions) == 0)
            {
                OnStatusApplicationFailed(checkContext);
                return ApplyStatusResult.NotAllowed;
            }

            // Find status if already applied
            AppliedStatusData statusReference = default;
            int statusReferenceIndex = -1;
            for (int i = 0; i < _appliedStatuses.Count; i++)
            {
                if (_appliedStatuses[i].status != status) continue;
                statusReference = _appliedStatuses[i];
                statusReferenceIndex = i;
                break;
            }

            // If status is not applied, apply it
            if (ReferenceEquals(statusReference.status, null))
            {
                StatusContext addStatusContext = new(this, status, stackCount);
                statusReference = new AppliedStatusData(status, stackCount);
                _appliedStatuses.Add(statusReference);
                OnStatusApplied(addStatusContext);
                return ApplyStatusResult.Success;
            }

            // If status is already applied, check if it can be stacked (or if max stack is reached)
            if (statusReference.stackCount >= status.MaxStack && status.MaxStack > 0 &&
                (flags & StatusModificationFlags.IgnoreStackLimit) == 0)
            {
                OnStatusApplicationFailed(checkContext);
                return ApplyStatusResult.MaxStackReached;
            }

            // If status can be stacked, stack it
            // StatusContext requires amount changed to be present rather than new value
            int stackChange = math.min(stackCount, status.MaxStack - statusReference.stackCount);
            StatusContext modifyStatusContext = new(this, status, stackChange);
            statusReference.stackCount += stackChange;
            OnStatusStackChanged(modifyStatusContext);

            // Refresh applied status reference as it's accessed as struct
            _appliedStatuses[statusReferenceIndex] = statusReference;
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
        /// <param name="flags">Flags to modify the removal</param>
        /// <returns>Result of the removal</returns>
        public RemoveStatusResult RemoveStatus(
            [NotNull] StatusBase status,
            int stackCount = 1,
            StatusModificationFlags flags = StatusModificationFlags.None)
        {
            // Get status removal context
            StatusContext checkContext = new StatusContext(this, status, stackCount);

            // Find status if already applied
            AppliedStatusData statusReference = default;
            int statusReferenceIndex = -1;
            for (int i = 0; i < _appliedStatuses.Count; i++)
            {
                if (_appliedStatuses[i].status != status) continue;

                statusReference = _appliedStatuses[i];
                statusReferenceIndex = i;
                break;
            }

            // If status is not applied, return invalid status
            if (ReferenceEquals(statusReference.status, null))
            {
                OnStatusRemovalFailed(checkContext);
                return RemoveStatusResult.NotApplied;
            }

            // Check if status can be removed
            if (!CanRemoveStatus(checkContext) && (flags & StatusModificationFlags.IgnoreConditions) == 0)
            {
                OnStatusRemovalFailed(checkContext);
                return RemoveStatusResult.NotAllowed;
            }

            // If status is applied, check if it can be removed
            if (statusReference.stackCount - stackCount < 0 &&
                (flags & StatusModificationFlags.IgnoreStackLimit) == 0)
            {
                OnStatusRemovalFailed(checkContext);
                return RemoveStatusResult.NotEnoughStacks;
            }

            // Remove stacks or clear status to zero if stack are overflown
            int stackChange = math.min(stackCount, statusReference.stackCount);
            statusReference.stackCount -= stackChange;

            // If status is now empty, remove it from the list
            if (statusReference.stackCount == 0 && statusReferenceIndex != -1)
            {
                StatusContext removeStatusContext = new(this, status, 0);
                OnStatusRemoved(removeStatusContext);

                // Remove status from list
                _appliedStatuses.RemoveAt(statusReferenceIndex);
            }
            else // If not, then handle stack reduction
            {
                StatusContext reduceStackContext = new(this, status, -stackChange);
                OnStatusStackChanged(reduceStackContext);

                // Update applied statuses
                _appliedStatuses[statusReferenceIndex] = statusReference;
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
            for (int i = 0; i < _appliedStatuses.Count; i++)
            {
                if (_appliedStatuses[i].status != status) continue;

                return _appliedStatuses[i].stackCount;
            }

            return 0;
        }

        /// <summary>
        ///     Performs per-tick status handling
        /// </summary>
        protected void HandleStatusTick(float deltaTime)
        {
            for (int i = 0; i < _appliedStatuses.Count; i++)
            {
                StatusContext tickContext = new(this, _appliedStatuses[i].status, _appliedStatuses[i].stackCount);
                _appliedStatuses[i].status.OnStatusTick(tickContext, deltaTime);
            }
        }

        /// <summary>
        ///     Check if status can be applied to the entity
        /// </summary>
        public virtual bool CanApplyStatus(in StatusContext context) => true;

        /// <summary>
        ///     Checks if status can be removed from the entity
        /// </summary>
        public virtual bool CanRemoveStatus(in StatusContext context) => true;

        /// <summary>
        ///     Executes when status is applied to the entity
        /// </summary>
        protected virtual void OnStatusApplied(in StatusContext context) =>
            context.status.OnStatusApplied(context);

        /// <summary>
        ///     Executes when status application fails
        /// </summary>
        protected virtual void OnStatusApplicationFailed(in StatusContext context) =>
            context.status.OnStatusApplicationFailed(context);

        /// <summary>
        ///     Executes when status is removed from the entity
        /// </summary>
        protected virtual void OnStatusRemoved(in StatusContext context) =>
            context.status.OnStatusRemoved(context);

        /// <summary>
        ///     Executes when status removal fails
        /// </summary>
        protected virtual void OnStatusRemovalFailed(in StatusContext context) =>
            context.status.OnStatusRemovalFailed(context);

        /// <summary>
        ///     Executes when status stack count changes
        /// </summary>
        protected virtual void OnStatusStackChanged(in StatusContext context) =>
            context.status.OnStatusStackChanged(context);

#endregion
    }
}