using System.Collections.Generic;
using System.Reflection;
using System;

using Object = UnityEngine.Object;
using UnityEditor;

namespace SFEditor.UIElements
{
    public static class ObjectPickerExtensions
    {
        public static void ShowObjectPicker<T>(this T initialValue, Action<Object> OnSelectorClosed, Action<Object> OnSelectionChanged) where T : Object
        {
            ShowObjectPicker<T>(OnSelectorClosed, OnSelectionChanged, initialValue);
        }

        public static void ShowObjectPicker<T>(Action<Object> OnSelectorClosed, Action<Object> OnSelectionChanged, T initialValueOrNull = null) where T : Object
        {
            Type[] methodParameterTypes = new Type[]{
                typeof(T),
                typeof(Type),
                typeof(Object),
                typeof(bool),
                typeof(List<int>),
                typeof(Action<Object>),
                typeof(Action<Object>),
                typeof(bool)
            };

            MethodInfo hiddenMethod = GetHiddenMethodInfo(typeof(Editor).Assembly, "UnityEditor", "ObjectSelector", "Show", methodParameterTypes, out object os);

            hiddenMethod.Invoke(os, new object[]
                {
                initialValueOrNull,
                typeof(T),
                null,
                false,
                null,
                OnSelectorClosed,
                OnSelectionChanged,
                true
                }
            );
        }

        /// <summary>
        /// Retrives the method info of any method or getter of a property, found inside the containing class that exists in the namesapce located in an assembly that has the parameters that match the array of types being passed in.
        /// </summary>
        /// <param name="assembly">This is the assembly of the class that contains the method you want to call.</param>
        /// <param name="classNamespace">The namesapce of the class containing your method.</param>
        /// <param name="className">The name of the class that contains the method you are calling.</param>
        /// <param name="methodName">The method you want to call.</param>
        /// <param name="parameterTypes">An array of parameters that are in the overload for the method you want call. This has to be exact in the order in the parameter overload. </param>
        /// <param name="os">This gets the values of the getter if the method is actual a property being called. It gets a value by the object class.</param>
        /// <returns>Return a method info object that can have invoke be called on  to invoke the hidden method while passing in any <see langword="set"/>of parameters for that method. </returns>
        public static MethodInfo GetHiddenMethodInfo(Assembly assembly, string classNamespace, string className, string methodName, Type[] parameterTypes, out object os)
        {
            var hiddenType = assembly.GetType($"{classNamespace}.{className}");
            os = hiddenType.GetProperty("get", BindingFlags.Public | BindingFlags.Static).GetValue(null);

            return hiddenType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, new ParameterModifier[0]);
        }

        public static MethodInfo GetHiddenMethodInfo(Assembly assembly, string classNamespace, string className, string methodName, Type[] parameterTypes)
        {
            var hiddenType = assembly.GetType($"{classNamespace}.{className}");

            return hiddenType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, new ParameterModifier[0]);
        }
    }
}
