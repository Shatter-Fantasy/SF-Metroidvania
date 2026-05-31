using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SFEditor.Utilities
{
    /// <summary>
    /// A collection of helper methods that uses Unity's built in <see cref="TypeCache"/>  
    /// </summary>
    public static class UnityEditorTypeUtilities
    {
        
        /// <summary>Search all assemblies for all types that match a predicate</summary>
        /// <param name="type">The type or interface to look for</param>
        /// <param name="predicate">Additional conditions to test</param>
        /// <returns>A list of types found that inherit from the type and satisfy the predicate.</returns>
        /// <example>
        /// <code>
        /// var allSources = ReflectionHelpers.GetTypesDerivedFrom(inputType,
        ///                     (t) => !t.IsAbstract and
        ///                     typeof(MonoBehaviour).IsAssignableFrom(t).GetCustomAttribute&lt;ObsoleteAttribute&gt;() == null);
        /// </code>
        /// </example>
        public static List<Type> GetTypesDerivedFrom(Type type, Predicate<Type> predicate)
        {
            var list = new List<Type>();
            if (predicate(type))
                list.Add(type);
            var iter = TypeCache.GetTypesDerivedFrom(type).GetEnumerator();
            while (iter.MoveNext())
            {
                var t = iter.Current;
                if (t != null && predicate(t))
                    list.Add(t);
            }
            return list;
        }
        
        public delegate MonoBehaviour ReferenceUpdater(Type expectedType, MonoBehaviour oldValue);

        /// <summary>
        /// Recursive scan that calls handler for all serializable fields that reference a MonoBehaviour
        /// </summary>
        public static bool RecursiveUpdateBehaviourReferences(GameObject go, ReferenceUpdater updater)
        {
            bool doneSomething = false;
            var components = go.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < components.Length; ++i)
            {
                var c = components[i];
                var obj = c as object;
                if (ScanFields(ref obj, updater))
                {
                    doneSomething = true;
                    if (UnityEditor.PrefabUtility.IsPartOfAnyPrefab(go))
                        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(c);
                }
            }
            return doneSomething;

            // local function
            static bool ScanFields(ref object obj, ReferenceUpdater updater)
            {
                if (obj == null)
                    return false;

                bool changed = false;

                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                if (obj is MonoBehaviour)
                    bindingFlags |= BindingFlags.NonPublic; // you can inspect non-public fields if thy have the attribute

                var fields = obj.GetType().GetFields(bindingFlags);
                for (int j = 0; j < fields.Length; ++j)
                {
                    var f = fields[j];

                    if (!f.IsPublic && f.GetCustomAttribute(typeof(SerializeField)) == null)
                        continue;

                    // Process the field
                    var type = f.FieldType;
                    if (typeof(MonoBehaviour).IsAssignableFrom(type))
                    {
                        var fieldValue = f.GetValue(obj);
                        var mb = fieldValue as MonoBehaviour;
                        if (mb != null)
                        {
                            var newValue = updater(type, mb);
                            if (newValue != mb)
                            {
                                changed = true;
                                f.SetValue(obj, newValue);
                            }
                        }
                    }

                    // Handle arrays and nested types
                    else if (type.IsArray)
                    {
                        if (f.GetValue(obj) is Array fieldValue)
                        {
                            for (int i = 0; i < fieldValue.Length; ++i)
                            {
                                var element = fieldValue.GetValue(i);
                                if (ScanFields(ref element, updater))
                                {
                                    fieldValue.SetValue(element, i);
                                    changed = true;
                                }
                            }
                            if (changed)
                                f.SetValue(obj, fieldValue);
                        }
                    }
                    else if (typeof(IList).IsAssignableFrom(type))
                    {
                        if (f.GetValue(obj) is IList fieldValue)
                        {
                            for (int i = 0; i < fieldValue.Count; ++i)
                            {
                                var element = fieldValue[i];
                                if (ScanFields(ref element, updater))
                                {
                                    fieldValue[i] = element;
                                    changed = true;
                                }
                            }
                            if (changed)
                                f.SetValue(obj, fieldValue);
                        }
                    }
                    else
                    {
                        // If the field type has fields of its own, process them
                        var fieldValue = f.GetValue(obj);
                        if (ScanFields(ref fieldValue, updater))
                            changed = true;
                    }
                }
                return changed;
            }
        }


    }
}
