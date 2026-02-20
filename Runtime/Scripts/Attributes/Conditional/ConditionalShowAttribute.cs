using System;
using UnityEngine;

namespace SF.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ConditionalShowAttribute : PropertyAttribute
    {
        public string PropertyName;
        public bool ShowInInspector;

        public ConditionalShowAttribute(bool showInInspector)
        {
            this.ShowInInspector = showInInspector;
        }
        public ConditionalShowAttribute(string propertyName, bool valueToMatch)
        {
            this.PropertyName = propertyName;
            this.ShowInInspector = valueToMatch;
        }
    }
}
