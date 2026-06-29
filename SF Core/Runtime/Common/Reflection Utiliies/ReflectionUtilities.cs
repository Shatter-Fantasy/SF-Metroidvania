using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SF.Utilities
{
    
    /* Credit and thank yous.
     * Thank you to Unity Cinemachine team for making some of the methods used here.
     * Some of these methods are actual built into Cinemachine package 3.1, but I am adding this here for those not using Cinemachine.
     */
    
    /// <summary>
    /// A collection of utility methods for general C# reflection.
    /// </summary>
    /// <remarks>
    /// For Unity specific Type utilities to use in editor scripts look at UnityEditorTypeUtilities inside the SFEditor assembly.
    /// </remarks>
    public static class ReflectionUtilities
    {
        /// <summary>Cheater extension to access internal field of an object</summary>
        /// <typeparam name="T">The field type</typeparam>
        /// <param name="type">The type of the field</param>
        /// <param name="obj">The object to access</param>
        /// <param name="memberName">The string name of the field to access</param>
        /// <returns>The value of the field in the objects</returns>
        public static T AccessInternalField<T>(this Type type, object obj, string memberName)
        {
            if (string.IsNullOrEmpty(memberName) || (type == null))
                return default;

            BindingFlags bindingFlags = BindingFlags.NonPublic;
            if (obj != null)
                bindingFlags |= BindingFlags.Instance;
            else
                bindingFlags |= BindingFlags.Static;

            FieldInfo field = type.GetField(memberName, bindingFlags);
            if ((field != null) && (field.FieldType == typeof(T)))
                return (T)field.GetValue(obj);
            return default;
        }
        
        /// <summary>Get the object owner of a field.  This method processes
        /// the '.' separator to get from the object that owns the compound field
        /// to the object that owns the leaf field</summary>
        /// <param name="path">The name of the field, which may contain '.' separators</param>
        /// <param name="obj">the owner of the compound field</param>
        /// <returns>The object owner of the field</returns>
        /// <remarks>
        /// The type of object mentioned here is C# lowercase object, not Unity's uppercase Object type. 
        /// </remarks>
        public static object GetParentObject(string path, object obj)
        {
            var fields = path.Split('.');
            if (fields.Length <= 1)
                return obj;

            var type = obj.GetType();
            if (type.IsArray || typeof(IList).IsAssignableFrom(type))
            {
                var elements = fields[1].Split('[');
                if (elements.Length > 1)
                {
                    var index = Int32.Parse(elements[1].Trim(']'));
                    if (type.IsArray)
                    {
                        if (obj is not Array a || a.Length <= index)
                            return null;
                        obj = a.GetValue(index);
                    }
                    else
                    {
                        var list = obj as IList;
                        if (list != null || list.Count <= index)
                            return null;
                        obj = list[index];
                    }
                    if (fields.Length <= 3)
                        return obj;
                }
            }
            else
            {
                var info = type.GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                obj = info.GetValue(obj);
            }
            return GetParentObject(string.Join(".", fields, 1, fields.Length - 1), obj);
        }

        /// <summary>Returns a string path from an expression - mostly used to retrieve serialized properties
        /// without hardcoding the field path. Safer, and allows for proper refactoring.</summary>
        /// <typeparam name="TType">Magic expression</typeparam>
        /// <typeparam name="TValue">Magic expression</typeparam>
        /// <param name="expr">Magic expression</param>
        /// <returns>The string version of the field path</returns>
        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    me = expr.Body as MemberExpression;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var members = new List<string>();
            while (me != null)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            var sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                sb.Append(members[i]);
                if (i > 0) sb.Append('.');
            }
            return sb.ToString();
        }
    }
}
