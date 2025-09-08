using UnityEngine;

namespace Systems.SimpleEntities.Components
{
    /// <summary>
    ///     Simple entity that is used to represent in-game objects
    /// </summary>
    public abstract class EntityBase : MonoBehaviour
    {
        protected virtual void AssignComponents()
        {
        }

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnEntitySetupComplete()
        {
        }

        protected virtual void OnEntityActivated()
        {
        }

        protected virtual void OnEntityDeactivated()
        {
        }

        protected virtual void OnTeardown()
        {
            
        }

#region Unity Lifecycle

        protected void Awake()
        {
            AssignComponents();
            OnInitialized();
        }

        protected void Start()
        {
            OnEntitySetupComplete();
        }

        protected void OnEnable()
        {
            OnEntityActivated();
        }

        protected void OnDisable()
        {
            OnEntityDeactivated();
        }

        private void OnDestroy()
        {
            OnTeardown();
        }

#endregion
    }
}