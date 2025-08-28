using Systems.SimpleEntities.Components;
using Systems.SimpleEntities.Examples.Affinity;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Entities
{
    /// <summary>
    ///     Example entity with fire and cold damage and resistance checks 
    /// </summary>
    public abstract class ExampleEntityBase : EntityBase
    {
        [ContextMenu("Deal fire damage")] public void DealFireDamage()
        {
            RefreshModifiersIfNecessary();
            Damage<FireAffinity>(this, MaxHealth);
            // or Damage(DamageContext.Create<FireAffinity>(this, this, MaxHealth));
        }

        [ContextMenu("Deal cold damage")] public void DealColdDamage()
        {
            RefreshModifiersIfNecessary();
            Damage<ColdAffinity>(this, MaxHealth);
            // or Damage(DamageContext.Create<ColdAffinity>(this, this, MaxHealth));
        }
    }
}