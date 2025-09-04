<div align="center">
  <h1>Simple Entities</h1>
</div>

# About

Simple Entities is a Simple Kit package for building gameplay actors and systems around them:

- Base entity lifecycle with activation hooks
- Ticking entities with optional fixed intervals and permission checks
- Alive entities with health, damage/heal flows, affinities and resistances
- Status effects with stacking, caps and per‑tick behavior

*For requirements check .asmdef*

# Usage

## Creating a basic entity

Extend `EntityBase` to get a minimal lifecycle with setup and activation hooks.

```csharp
using Systems.SimpleEntities.Components;
using UnityEngine;

public sealed class ChestEntity : EntityBase
{
    protected override void AssignComponents() { /* cache refs */ }
    protected override void OnInitialized() { /* one-time init */ }
    protected override void OnEntitySetupComplete() { /* after Start */ }
    protected override void OnEntityActivated() { /* OnEnable */ }
    protected override void OnEntityDeactivated() { /* OnDisable */ }
}
```

## Ticking entities

Use `TickingEntityBase` for per-frame or interval-based logic. You can gate ticking with `CanTimePass` and `CanTick`.

```csharp
using Systems.SimpleCore.Operations;
using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Operations;

public sealed class SpawnerEntity : TickingEntityBase
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        TickInterval = 1.0f; // tick once per second
    }

    public override bool CanTimePass() => true; // pause control
    public override OperationResult CanTick() => EntityOperations.Permitted();

    protected override void OnTick(float deltaTime)
    {
        // spawn logic here
    }
}
```

You can also trigger manual ticks via `ExecuteTick(deltaTime, flags)`.

## Alive entities (health, damage, healing)

`AliveEntityBase` adds health, damage/heal flows, status handling, and stat modifiers.

```csharp
using Systems.SimpleCore.Utility.Enums;          // ActionSource
using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Data.Affinity;
using Systems.SimpleEntities.Data.Context;

public sealed class EnemyEntity : AliveEntityBase
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        MaxHealth = 1000;
        CurrentHealth = MaxHealth;
    }

    // Optional: gate damage/heal by affinity
    public override OperationResult CanBeDamaged(in DamageContext ctx) => base.CanBeDamaged(in ctx);
    public override OperationResult CanBeHealed(in HealContext ctx) => base.CanBeHealed(in ctx);
}

// Damaging with an affinity type
var result = enemy.Damage<PhysicalAffinity>(source: this, amount: 120, ActionSource.External);
bool ok = result;                 // success?
long dealt = (long)result;        // health lost

// Healing with an affinity type
var healed = enemy.Heal<HealingAffinity>(source: this, amount: 50);
```

Death flow: when damage reduces health to 0, `Kill` is invoked. Override `CanSaveFromDeath` to implement death saves and set post‑save health.

## Resistances and affinities

Define `AffinityType` assets and `ResistanceBase` stats to influence damage/heal calculations. `GetResistance<TAffinity>()` aggregates all modifiers for that affinity via `SimpleStats`.

```csharp
float fireResist = enemy.GetResistance<FireAffinity>();
```

## Status effects

Statuses are ScriptableObjects extending `StatusBase` with application/removal rules, stacking and per‑tick behavior.

```csharp
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleEntities.Data.Enums;   // StatusModificationFlags
using Systems.SimpleEntities.Data.Status.Abstract;

// Apply, stack and remove statuses
var applied = enemy.ApplyStatus<BurningStatus>(stackCount: 2);
int newStacks = (int)applied;              // resulting stack count

var stacked = enemy.ApplyStatus<BurningStatus>(1, StatusModificationFlags.IgnoreConditions);
var removed = enemy.RemoveStatus<BurningStatus>(1);

bool hasBurn = enemy.HasStatus<BurningStatus>();
int burnStacks = enemy.GetStatusStackCount<BurningStatus>();
```

Event hooks on `AliveEntityBase` proxy to the status asset by default, allowing central logic in the status definition:

- On applied/failed, removed/failed, stack changed
- Per‑tick via `OnStatusTick(context, deltaTime)`

## Operations

Common operation helpers are in `EntityOperations` and `StatusOperations` using `OperationResult` from SimpleCore:

- Entity: `Damaged()`, `Healed()`, `Killed()`, `SavedFromDeath()`, `TickExecuted()`, `Permitted()`
- Status: e.g., `StatusApplied()`, `StatusRemoved()`, `StatusStackChanged()`, `NotApplied()`, `NotEnoughStacks()`, `MaxStackReached()`

# Notes

- Use `ActionSource.Internal` for silent system changes; `External` to fire events.
- For interval ticking, set `TickInterval > 0` or handle logic per frame in `OnTick` when `TickInterval == 0`.
- Integrates with `SimpleStats` for resistances and modifiers; ensure relevant stats are present in your database.

