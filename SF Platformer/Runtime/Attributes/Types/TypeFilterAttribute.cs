using System;

using Systems;

using UnityEngine;

namespace SF
{
    public class TypeFilterAttribute : PropertyAttribute
    {
        public Func<Type,bool> Filter { get; }

        public TypeFilterAttribute(Type filterType)
        {
            Filter = type => !type.IsAbstract && 
                             !type.IsInterface &&
                             !type.IsGenericType &&
                             type.InheritOrImplements(filterType);
        }
    }
}
