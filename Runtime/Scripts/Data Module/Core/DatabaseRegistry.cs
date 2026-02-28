using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SF.DataModule
{
    [CreateAssetMenu(fileName = nameof(DatabaseRegistry), menuName = "SF/Data/Database Registry")]
    public class DatabaseRegistry : ScriptableObject
    {
        /// <summary>
        /// A list of databases needing to be preloaded when the runtime player first starts up.
        /// Any database set in here will have the 
        /// </summary>
        public List<SFDatabase> PreloadedDatabase = new List<SFDatabase>();
        public Dictionary<Type, SFDatabase> RegisteredDatabases = new();

        private static DatabaseRegistry _registry;

        public static DatabaseRegistry Registry
        {
            get
            {
                if (_registry == null)
                    _registry = CreateInstance<DatabaseRegistry>();

                return _registry;
            }
            /* No setter because when grabbing the registry for the first time it will auto set an instance.
             * if none was already set during an Awake call for a scriptable object of type DatabaseRegistry */
        }

        private void Awake()
        {
            // If this is the first time we created a registry set it as the default to prevent null values.
            if (_registry != null)
                return;

            _registry = this;
        }

        private void OnEnable()
        {
            for (int i = 0; i < PreloadedDatabase.Count; i++)
            {
                RegisterDatabase(PreloadedDatabase[i]);
            }
        }
        
        private void OnDestroy()
        {
            for (int i = 0; i < PreloadedDatabase.Count; i++)
            {
                DeregisterDatabase(PreloadedDatabase[i]);
            }
        }
        
        public static bool Contains(Type databaseType)
        {
            return _registry.RegisteredDatabases.ContainsKey(databaseType);
        }

        public static TDatabase GetDatabase<TDatabase>() where TDatabase : SFDatabase
        {
            _registry.RegisteredDatabases.TryGetValue(typeof(TDatabase), out var database);
            return (TDatabase)database;
        }

        public static void RegisterDatabase<TDatabase>(TDatabase database) where TDatabase : SFDatabase
        {
            // We use the Registry property with the getter just in
            // case we need to create an instance before registering a database.
            if (!Registry.RegisteredDatabases.TryAdd(database.GetType(),database))
            { 
#if UNITY_EDITOR
                Debug.LogWarning($"When registering a database of type: {database.GetType()}, there was one already registered. Only one of each type can be registered at once.");
#endif
            }
        }
        
        public static void DeregisterDatabase<TDatabase>(TDatabase database) where TDatabase : SFDatabase
        {
            // We use the Registry property with the getter just in
            // case we need to create an instance before even attempting to do unregistering.
            if (!Registry.RegisteredDatabases.Remove(typeof(TDatabase)))
            { 
#if UNITY_EDITOR
                Debug.LogWarning($"When unregistering a database of type: {typeof(TDatabase)}, there was no registered database of that type.");
#endif
            }
        }
        
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("SF/Data/Register Preloaded Databases")]
        public static void PreloadDatabases()
        {
            var databaseRegistry = DatabaseRegistry.Registry;

            if (databaseRegistry == null)
            {
                Debug.Log("There was not DatabaseRegistry set as the active registry.");
                return;
            }

            // Add the config asset to the build
            var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();
            
            // Don't set it if it already is in the PreloadedAssets list.
            if (preloadedAssets.Contains(databaseRegistry))
                return;
            
            preloadedAssets.Add(databaseRegistry);
            UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
#endif
    }
}
