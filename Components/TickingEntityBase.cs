using Systems.SimpleEntities.Data.Enums;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.SimpleEntities.Components
{
    /// <summary>
    ///     Entity that ticks every frame
    /// </summary>
    public abstract class TickingEntityBase : EntityBase
    {
        /// <summary>
        ///     Timer for tick interval
        /// </summary>
        private float _tickTimer;

        /// <summary>
        ///     Tick interval, if less or equal to 0, ticks every frame
        /// </summary>
        protected float TickInterval { get; set; } = 0f;

        /// <summary>
        ///     Executes tick event, useful for turn-based systems
        /// </summary>
        /// <param name="flags">Flags for tick execution</param>
        /// <param name="deltaTime">Time passed since last tick</param>
        public void ExecuteTick(float deltaTime, EntityTickFlags flags = EntityTickFlags.None)
        {
            // Skip if tick cannot be performed, otherwise execute
            if (!CanTick() && (flags & EntityTickFlags.ForceTick) == 0) return;
            OnTick(deltaTime);

            // Update timer and limit to 0
            _tickTimer -= deltaTime;
            _tickTimer = math.max(_tickTimer, 0f);
        }

        internal void HandleEntityTick()
        {
            float timePassedSeconds = Time.deltaTime;

            // Skip if time cannot pass
            if (!CanTimePass()) return;

            if (TickInterval <= 0f)
                OnTick(timePassedSeconds);
            else
            {
                _tickTimer += timePassedSeconds;

                // Handle interval passed, skip if tick cannot be performed
                // execute for all ticks that completed on this frame
                while (_tickTimer >= TickInterval) ExecuteTick(TickInterval);
            }
        }

#region Checks

        /// <summary>
        ///     If true time updates can be performed
        /// </summary>
        public virtual bool CanTimePass() => true;

        /// <summary>
        ///     If true tick event can be performed
        /// </summary>
        /// <remarks>
        ///     If time can pass, but tick cannot, it will be executed at next frame
        ///     when tick execution possibility is true
        /// </remarks>
        public virtual bool CanTick() => true;

#endregion

#region Events

        protected virtual void OnTick(float deltaTime)
        {
        }

#endregion
    }
}