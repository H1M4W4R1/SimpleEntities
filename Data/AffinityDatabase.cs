using System.Collections.Generic;
using JetBrains.Annotations;
using Systems.SimpleEntities.Affinity;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Systems.SimpleEntities.Data
{
    /// <summary>
    ///     Database of all damage affinities in game
    /// </summary>
    public static class AffinityDatabase
    {
        public const string ADDRESSABLE_LABEL = "SimpleEntities.Affinity";
        private static readonly List<DamageAffinity> _items = new();

        /// <summary>
        ///     If true this means that all objects have been loaded
        /// </summary>
        private static bool _isLoaded;

        /// <summary>
        ///     If true this means that objects are currently being loaded
        /// </summary>
        private static bool _isLoading;

        /// <summary>
        ///     Total number of objects in database
        /// </summary>
        public static int Count
        {
            get
            {
                EnsureLoaded();
                return _items.Count;
            }
        }

        /// <summary>
        ///     Ensures that all objects are loaded
        /// </summary>
        internal static void EnsureLoaded()
        {
            if (!_isLoaded) Load();
        }

        /// <summary>
        ///     Loads all objects from Resources folder
        /// </summary>
        private static void Load()
        {
            // Prevent multiple loads
            if (_isLoading) return;
            _isLoading = true;

            // Load items
            AsyncOperationHandle<IList<DamageAffinity>> request = Addressables.LoadAssetsAsync<DamageAffinity>(
                new[] {ADDRESSABLE_LABEL}, OnItemLoaded,
                Addressables.MergeMode.Union);
            request.WaitForCompletion();

            OnItemsLoadComplete(request);
        }

        private static void OnItemsLoadComplete(AsyncOperationHandle<IList<DamageAffinity>> _)
        {
            _isLoaded = true;
            _isLoading = false;
        }

        private static void OnItemLoaded<TObject>(TObject obj)
        {
            if (obj is not DamageAffinity item) return;
            _items.Add(item);
        }


        /// <summary>
        ///     Gets first object of specified type
        /// </summary>
        /// <typeparam name="TDamageAffinity">Object type to get </typeparam>
        /// <returns>First object of specified type or null if no object of specified type is found</returns>
        [CanBeNull] public static TDamageAffinity GetAffinity<TDamageAffinity>()
            where TDamageAffinity : DamageAffinity
        {
            EnsureLoaded();

            // Loop through all items
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] is TDamageAffinity item) return item;
            }

            Assert.IsNotNull(null, "Item not found in database");
            return null;
        }

        /// <summary>
        ///     Gets all objects of specified type
        /// </summary>
        /// <typeparam name="TDamageAffinity">Type of object to get</typeparam>
        /// <returns>Read-only list of objects of specified type</returns>
        [NotNull] public static IReadOnlyList<TDamageAffinity> GetAll<TDamageAffinity>()
            where TDamageAffinity : DamageAffinity
        {
            EnsureLoaded();

            List<TDamageAffinity> items = new();

            // Loop through all items
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] is TDamageAffinity item) items.Add(item);
            }

            return items;
        }
    }
}