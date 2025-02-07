using System.Runtime.CompilerServices;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Diagnostics;

namespace System.Runtime.InteropServices
{
    /* DON"T TOUCH THIS FILE OR THE WORLD WILL BE DESTROYED.*/
    /* DON"T TOUCH THIS FILE OR THE WORLD WILL BE DESTROYED.*/
    /* DON"T TOUCH THIS FILE OR THE WORLD WILL BE DESTROYED.*/

    /* This entire file brings .net 9 and CoreCLR stuff not normally in Unity into the version of .net that Unity uses.*/


    /// <summary>
    /// Used internally to control behavior of insertion into a <see cref="Dictionary{TKey, TValue}"/> or <see cref="HashSet{T}"/>.
    /// </summary>
    internal enum InsertionBehavior : byte
    {
        /// <summary>
        /// The default insertion behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that an existing entry with the same key should be overwritten if encountered.
        /// </summary>
        OverwriteExisting = 1,

        /// <summary>
        /// Specifies that if an existing entry with the same key is encountered, an exception should be thrown.
        /// </summary>
        ThrowOnExisting = 2
    }

    /// <summary>
    /// Used by ArgumentExecption throwers.
    /// </summary>
    internal enum ExceptionArgument
    {
        buffer,
        offset,
        length,
        text,
        start,
        count,
        index,
        value,
        capacity,
        separators,
        comparisonType,
        changeTokens,
        changeTokenProducer,
        changeTokenConsumer,
        array,
    }

    public static class CollectionsMarshal
    {
        public static Span<T> AsSpan<T>(List<T> list)
        {
            if(list == null)
                return default(Span<T>);


            var box = new ListCastHelper { List = list }.StrongBox;
            return new Span<T>((T[])box.Value, 0, list.Count);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct ListCastHelper
    {
        [FieldOffset(0)]
        public StrongBox<Array> StrongBox;

        [FieldOffset(0)]
        public object List;
    }
}