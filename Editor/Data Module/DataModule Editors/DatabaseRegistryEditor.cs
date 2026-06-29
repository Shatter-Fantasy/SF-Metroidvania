using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace SFEditor.DataModule
{
    using SF.DataModule;
    using static Utilities.AssetDatabaseEditorUtilities;
    
    [InitializeOnLoad]
    public static class DatabaseRegistryCompiler
    {
        static DatabaseRegistryCompiler()
        {
            /* We call OnCompilationFinished instead of doing the asset database loading directly here
                because Unity's asset database might not have finished importing the assets into the project on first opening the project.            */
            CompilationPipeline.compilationFinished += OnCompilationFinished;
        }

        [InitializeOnLoadMethod]
        private static void OnLoadCompleted()
        {
            if (DatabaseRegistry.Registry == null)
            {
                var registry = FindFirstAssetOfType<DatabaseRegistry>();
                if (registry != null)
                    DatabaseRegistry.Registry = registry;
            }
        }
        private static void OnCompilationFinished(object obj)
        {
            if (DatabaseRegistry.Registry == null)
            {
                var registry = FindFirstAssetOfType<DatabaseRegistry>();
                if (registry == null)
                {
                    Debug.Log($"There was no Database Registry created in the project folder.");
                }
                else
                {
                    DatabaseRegistry.Registry = registry;
                    Debug.Log($"Database Registry was assigned.");
                }
            }
        }
    }
}
