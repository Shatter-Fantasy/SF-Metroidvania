using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel.Utilities
{
    /*  Special thanks to MelvMay, the creator of Unity's low level 2D physics system. He personally created these method during the Unity 6.3 beta to help give examples.
        Check out the link below for the public repo where he helped show examples on how to do this.
        https://github.com/Unity-Technologies/PhysicsExamples2D/tree/master/LowLevel/Projects/Snippets/Assets/Examples/ContactFilteringExtensions    
         */
    
    public static class ContactExtensions
    {
        /// <summary>
        /// A delegate used as a filter for contacts.
        /// The PhysicsShape is passed to the filter function which can be used if filtering the normal as it is always returned as the normal from shape A to shape B.
        /// </summary>
        public delegate bool ContactFilterFunction(ref PhysicsShape.Contact contact, PhysicsShape shapeContext);
        
        /// <summary>
        /// Enumerate an array of contacts based upon a filter function.
        /// </summary>
        /// <param name="contacts">The array of contacts to enumerate.</param>
        /// <param name="filterFunction">The filter function to use to check each element.</param>
        /// <param name="shapeContext">The shape context to pass to the filter function. Used when filtering the normal.</param>
        /// <returns>An enumerable set of contacts filtered by the filter function.</returns>
        public static IEnumerable<PhysicsShape.Contact> Filter(this NativeArray<PhysicsShape.Contact> contacts,
            ContactFilterFunction filterFunction, PhysicsShape shapeContext = default) =>
            new FilteredContacts(contacts, filterFunction, shapeContext);
        
        
        private struct FilteredContacts : IEnumerable<PhysicsShape.Contact>, IEnumerator<PhysicsShape.Contact>
        {

            public FilteredContacts(NativeArray<PhysicsShape.Contact> contacts, ContactFilterFunction filterFunction,
                PhysicsShape shapeContext)
            {
                m_Contacts       = contacts;
                m_ShapeContext   = shapeContext;
                m_FilterFunction = filterFunction;
                m_Index          = -1;
            }
            
     

            private readonly NativeArray<PhysicsShape.Contact> m_Contacts;
            private readonly PhysicsShape m_ShapeContext;
            private readonly ContactFilterFunction m_FilterFunction;
            private int m_Index;

#region Enumeration

            object IEnumerator.Current => m_Contacts[m_Index];
            PhysicsShape.Contact IEnumerator<PhysicsShape.Contact>.Current => m_Contacts[m_Index];

            bool IEnumerator.MoveNext()
            {
                // Filter the contacts.
                while (++m_Index < m_Contacts.Length)
                {
                    var contact = m_Contacts[m_Index];
                    if (m_FilterFunction(ref contact, m_ShapeContext))
                        return true;
                }

                return false;
            }

            void IEnumerator.Reset() => m_Index = -1;

            public IEnumerator<PhysicsShape.Contact> GetEnumerator() =>
                new FilteredContacts(m_Contacts, m_FilterFunction, m_ShapeContext);

            IEnumerator IEnumerable.GetEnumerator() =>
                new FilteredContacts(m_Contacts, m_FilterFunction, m_ShapeContext);

            // Iterator does not own the buffer, nothing to dispose.
            public readonly void Dispose()
            {
            }

#endregion
        }
        
        #region Normal Filtering
        public delegate bool ContactNormalFilterFunction(
            ref PhysicsShape.Contact contact, 
            PhysicsShape shapeContext,
            float normalizedXValue = 0f, 
            FilterMathOperator filterMathOperator = FilterMathOperator.Equal);
        
        /// <summary>
        /// Enumerate an array of contacts based upon a filter function.
        /// </summary>
        /// <param name="contacts">The array of contacts to enumerate.</param>
        /// <param name="filterFunction">The filter function to use to check each element.</param>
        /// <param name="shapeContext">The shape context to pass to the filter function. Used when filtering the normal.</param>
        /// <param name="normalizedValue"></param>
        /// <param name="filterMathOperator"></param>
        /// <returns>An enumerable set of contacts filtered by the filter function.</returns>
        public static IEnumerable<PhysicsShape.Contact> Filter(
            this NativeArray<PhysicsShape.Contact> contacts,
            ContactNormalFilterFunction filterFunction,
            PhysicsShape shapeContext = default,
            float normalizedValue = 0f, 
            FilterMathOperator filterMathOperator = FilterMathOperator.Equal) =>
            new FilteredNormalContacts(contacts, filterFunction, shapeContext, normalizedValue, filterMathOperator);
        
        /// <summary>
        /// Create a list of contacts based upon a filter function.
        /// </summary>
        /// <param name="contacts">The array of contacts to enumerate.</param>
        /// <param name="filterFunction">The filter function to use to check each element.</param>
        /// <param name="shapeContext">The shape context to pass to the filter function. Used when filtering the normal.</param>
        /// <param name="allocator">The allocator to use to create the list.</param>
        /// <returns>A native list of contacts filtered by the filter function.</returns>
        public static NativeList<PhysicsShape.Contact> ToFilteredList(this NativeArray<PhysicsShape.Contact> contacts,
            ContactFilterFunction filterFunction, PhysicsShape shapeContext = default,
            Allocator allocator = Allocator.Temp)
        {
            var filterList = new NativeList<PhysicsShape.Contact>(initialCapacity: contacts.Length, allocator);

            // Filter the contacts.
            foreach (var contact in new FilteredContacts(contacts, filterFunction, shapeContext))
                filterList.Add(contact);

            return filterList;
        }
        
        // Enumerate contacts via a user-defined contact filter function.
        private struct FilteredNormalContacts : IEnumerable<PhysicsShape.Contact>, IEnumerator<PhysicsShape.Contact>
        {
            private readonly NativeArray<PhysicsShape.Contact> _contacts;
            private readonly PhysicsShape _shapeContext;
            private readonly ContactNormalFilterFunction _normalFilterFunction;
            private float _normalizedValue;
            private FilterMathOperator _filterMathOperator;
            private int _index;
            
            public FilteredNormalContacts(NativeArray<PhysicsShape.Contact> contacts,
                    ContactNormalFilterFunction filterFunction,
                    PhysicsShape shapeContext,
                    float normalizedValue = 0f, 
                    FilterMathOperator filterMathOperator = FilterMathOperator.Equal
                )
            {
                _contacts             = contacts;
                _shapeContext         = shapeContext;
                _normalFilterFunction = filterFunction;
                _normalizedValue      = normalizedValue;
                _filterMathOperator   = filterMathOperator;
                _index                = -1;
            }
            
#region Enumeration

            object IEnumerator.Current => _contacts[_index];
            PhysicsShape.Contact IEnumerator<PhysicsShape.Contact>.Current => _contacts[_index];

            bool IEnumerator.MoveNext()
            {
                // Filter the contacts.
                while (++_index < _contacts.Length)
                {
                    var contact = _contacts[_index];
                    if (_normalFilterFunction(ref contact, _shapeContext,_normalizedValue,_filterMathOperator))
                        return true;
                }

                return false;
            }

            void IEnumerator.Reset() => _index = -1;

            public IEnumerator<PhysicsShape.Contact> GetEnumerator() =>
                new FilteredNormalContacts(_contacts, _normalFilterFunction, _shapeContext,_normalizedValue,_filterMathOperator);

            IEnumerator IEnumerable.GetEnumerator() =>
                new FilteredNormalContacts(_contacts, _normalFilterFunction, _shapeContext,_normalizedValue,_filterMathOperator);

            // Iterator does not own the buffer, nothing to dispose.
            public readonly void Dispose()
            {
            }

#endregion
        }
        
#endregion
    }
}
