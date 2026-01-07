using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.DataModule
{
    [CreateAssetMenu(fileName = nameof(DatabaseRegistry), menuName = "SF/Data/Database Registry")]
    public class DatabaseRegistry : ScriptableObject
    {
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
             * if none was already set during an Awake cvall for a scriptable object of type DatabaseRegistry */
        }

        private void Awake()
        {
            // If this is the first time we created a registry set it as the default to prevent null values.
            if (_registry != null)
                return;

            _registry = this;
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
        
        public static void UnregisterDatabase<TDatabase>() where TDatabase : SFDatabase
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
    }
}
