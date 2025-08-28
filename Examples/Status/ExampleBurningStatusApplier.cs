using System;
using Systems.SimpleEntities.Components;
using UnityEngine;

namespace Systems.SimpleEntities.Examples.Status
{
    [RequireComponent(typeof(EntityBase))]
    public sealed class ExampleBurningStatusApplier : MonoBehaviour
    {
        private EntityBase _entity;

        private void Awake()
        {
            _entity = GetComponent<EntityBase>();
        }

        [ContextMenu("Set on flame")] private void SetOnFlame()
        {
            _entity.ApplyStatus<BurningStatusExample>();
        }

        [ContextMenu("Remove from flame")] private void RemoveFromFlame()
        {
            _entity.RemoveStatus<BurningStatusExample>();
        }

        [ContextMenu("Check if is on flame")] private void CheckIfIsOnFlame()
        {
            bool isOnFlame = _entity.HasStatus<BurningStatusExample>();
            Debug.Log(isOnFlame ? $"{_entity.name} is on flame!" : $"{_entity.name} is not on flame!");
        }
    }
}