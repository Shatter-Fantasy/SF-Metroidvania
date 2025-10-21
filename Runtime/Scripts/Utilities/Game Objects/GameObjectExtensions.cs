using UnityEngine;

namespace SF.Utilities
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInChildren<T>();

            return component is not null;
        }

        public static bool TryGetComponentInChildren<T>(this Component gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponentInChildren<T>();

            return component is not null;
        }
    }
}
