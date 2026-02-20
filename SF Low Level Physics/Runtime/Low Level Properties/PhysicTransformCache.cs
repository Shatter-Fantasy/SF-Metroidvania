#if UNITY_EDITOR && UNITY_6000_5_OR_NEWER
/*In Unity 6.5 the PhysicsWorld.RegisterTransformChange was introduced and
 this entire class is no longer needed in Unity 6.5 and newer.
 */
#elif UNITY_EDITOR 

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Informs any implemented object that a <see cref="Transform"/> has been changed.
    /// </summary>
    public interface ITransformMonitor
    {
        void TransformChanged();
    }

    /// <summary>
    /// Keeps track of when <see cref="UnityEngine.Transform"/> needs to inform an active <see cref="SFShapeComponent.Body"/>
    /// to update it's <see cref="UnityEngine.LowLevelPhysics2D.PhysicsTransform"/>
    /// <remarks>
    /// In Unity 6.5 the PhysicsWorld.RegisterTransformChange was introduced and
    /// this entire class is no longer needed in Unity 6.5 and newer.
    /// </remarks>
    /// </summary>
    public static class PhysicTransformCache
    {
        private static readonly Dictionary<Transform, HashSet<ITransformMonitor>> Monitors = new();
        private static readonly List<Transform> ChangedTransforms = new();
        private static readonly List<ITransformMonitor> BufferedCallbacks = new();
            
        [InitializeOnLoadMethod]
        public static void InitializeAllWatchers()
        {
            Monitors.Clear();
            ChangedTransforms.Clear();
            EditorApplication.update += MonitorUpdate;
        }

        private static void MonitorUpdate()
        {
            // We only do this in edit mode.
            if (EditorApplication.isPlaying)
                return;

            // Check for transform changes.
            foreach (var monitor in Monitors)
            {
                // Skip if the transform hasn't changed.
                var transform = monitor.Key.transform;
                if (!transform.hasChanged)
                    continue;

                // Add to transforms that have changed.
                // We may get duplicates here, but it doesn't matter.
                ChangedTransforms.Add(transform);

                // Fetch the buffered callbacks.
                // NOTE: We do this because the callbacks can have side effects such as changing the callback enumeration.
                BufferedCallbacks.Clear();
                BufferedCallbacks.AddRange(monitor.Value);
                
                // Call the monitors.
                foreach (var callback in BufferedCallbacks)
                    callback.TransformChanged();
            }

            // Reset changed transforms.
            foreach (var transform in ChangedTransforms)
                transform.hasChanged = false;

            // Reset the list.
            ChangedTransforms.Clear();
        }

        public static void AddMonitor(Component component) => AddMonitor(component.transform, component as ITransformMonitor);
        
        public static void AddMonitor(Transform transform, ITransformMonitor callback)
        {
            if (transform == null)
                throw new NullReferenceException(nameof(transform));
            
            if (callback == null)
                throw new NullReferenceException(nameof(callback));

            if (Monitors.TryGetValue(transform, out var callbacks))
            {
                callbacks.Add(callback);
                return;
            }

            // Add a new callback.
            var newCallbacks = HashSetPool<ITransformMonitor>.Get();
            newCallbacks.Add(callback);
            Monitors.Add(transform, newCallbacks);
        }

        public static void RemoveMonitor(Transform transform, ITransformMonitor callback)
        {
            if (transform == null)
                throw new NullReferenceException(nameof(transform));
            
            if (callback == null)
                throw new NullReferenceException(nameof(callback));

            // Finish if there's no monitors found.
            if (!Monitors.TryGetValue(transform, out var callbacks))
                return;
            
            // Remove the callback.
            callbacks.Remove(callback);

            // Finish if callbacks still exist.
            if (callbacks.Count > 0)
                return;
            
            // Release the callbacks.
            HashSetPool<ITransformMonitor>.Release(callbacks);
            
            // Remove from the monitors.
            Monitors.Remove(transform);
        }
        
        public static void RemoveMonitor(Component component) => RemoveMonitor(component.transform, component as ITransformMonitor);
    }
}
#endif