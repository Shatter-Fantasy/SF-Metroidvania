using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SF.Utilities.InteropServices
{

    public static class CollectionsMarshal
    {
        public static Span<T> AsSpan<T>(List<T> list)
        {
            if(list == null)
                return default(Span<T>);

            var box = new ListCastHelper { List = list }.StrongBox;
            return new Span<T>((T[])box.Value, 0, list.Count);
        }


        /// <summary>
        /// Used to help lock data from arrays and lists in spot to be convertable into Spans.
        /// Since Spans behind the scenes run faster due to locking spaces of memory in spot we need to 
        /// to tell the compiler to explicilty declare a layout for the struct memory.
        /// Without doing this when converting Arrays, Lists, certain other collection types using <see cref="StrongBox"/> into a Span the memory will be messed up when read back in the Span.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct ListCastHelper
        {
            [FieldOffset(0)]
            public StrongBox<Array> StrongBox;

            [FieldOffset(0)]
            public object List;
        }
    }

}
