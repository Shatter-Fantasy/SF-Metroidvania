using UnityEngine;

namespace SF
{
    public static class ComponentUtilities
    {
        #region Behavior Specific 
        public static bool IsActiveEnabledAndNotNull<T>(this T behaviour) where T : Behaviour
        {
            return behaviour != null && behaviour.isActiveAndEnabled;
        }
        #endregion
    }
}
