using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SF.UIElements.Utilities
{
    public static class ButtonElementsExtensions
    {
        /* Start of the event registering extensions. */
        public static T OnClick<T>(this T target, Action callback)
            where T : Button
        {
            if (target == null)
            {
                Debug.LogWarning("When trying to register a Button OnClick event, the target Button element was null.");
            }
            
            target?.RegisterCallback<MouseUpEvent>((evt) => callback());
            return target;
        }
    }
}
