using System;
using System.Linq;

namespace Systems
{
    public static class TypeExtensions 
    {
        static Type ResolveGenericType(Type type)
        {
            if(type is not { IsGenericType: true }) return type;

            var genericType = type.GetGenericTypeDefinition();
            return genericType != type ? genericType : type;
        }

        static bool HasAnyInterfaces(Type type, Type interfaceTyp)
        {
            return type.GetInterfaces().Any(iType => ResolveGenericType(iType) == interfaceTyp);
        }

        /// <summary>
        /// Checks if a given type inherits or implements a specified base type.
        /// </summary>
        /// <param name="type"></param> The type which needs to be checked.
        /// <param name="baseType"></param> The base type/interface which is expected to be inherited or implemented 
        /// by the type.
        /// <returns> Return true if 'type' inherits or implements 'baseType'. False otherwise.</returns>
        public static bool InheritOrImplements(this Type type, Type baseType)
        {
            type = ResolveGenericType(type);
            baseType = ResolveGenericType(baseType);

            while(type != typeof(object))
            {
                if(baseType == type || HasAnyInterfaces(type, baseType)) return true;

                type = ResolveGenericType(type.BaseType);
                if(type == null) return false;
            }

            return false;
        }
    }
}
