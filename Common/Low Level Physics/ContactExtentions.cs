using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.LowLevelPhysics2D;

namespace SF.PhysicsLowLevel
{
    /*  Special thanks to MelvMay, the creator of Unity's low level 2D physics system. He personally created these method during the Unity 6.3 beta to help give examples.
        Check out the link below for the public repo where he helped show examples on how to do this.
        https://github.com/Unity-Technologies/PhysicsExamples2D/tree/master/LowLevel/Projects/Snippets/Assets/Examples/ContactFilteringExtensions    
         */
    
    public static class ContactExtensions
    {
        public enum FilterFunctionMode
        {
            NormalVector = 0, // Using the Vector2 normalized value for a filter.
            NormalAngle = 1,
            NormalImpulse = 2,
            NormalAngleAndImpulse = 4
        }
    
        public enum FilterMathOperator
        {
            GreaterThan = 0, 
            LessThan = 1,
            Equal = 2,
            GreaterThanOrEqual = 4,
            LessThanOrEqual = 8,
            NotEqual = 16
        }

        /// <summary>
        /// Used to choose which value or values of a Vector2 should a filter operation be done on. 
        /// </summary>
        public enum FilterVector2Operator
        {
            BothXY = 0, // Both have to pass the filter predicate check.
            JustX = 1,
            JustY = 2,
            EitherXY = 4, // Only one needs to pass the filter predicate check.
        }

        
        /// <summary>
        /// A delegate used as a filter for contacts.
        /// The PhysicsShape is passed to the filter function which can be used if filtering the normal as it is always returned as the normal from shape A to shape B.
        /// </summary>
        public delegate bool ContactFilterFunction(ref PhysicsShape.Contact contact, PhysicsShape shapeContext);
        
        public delegate bool ContactNormalVectorFilterFunction(
            ref PhysicsShape.Contact contact, 
            PhysicsShape shapeContext,
            FilterMathOperator filterMathOperator, 
            FilterVector2Operator filterVector2Operator);
        
        

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
        
        /// <summary>
        /// Enumerate an array of contacts based upon a filter function.
        /// </summary>
        /// <param name="contacts">The array of contacts to enumerate.</param>
        /// <param name="filterFunction">The filter function to use to check each element.</param>
        /// <param name="shapeContext">The shape context to pass to the filter function. Used when filtering the normal.</param>
        /// <returns>An enumerable set of contacts filtered by the filter function.</returns>
        public static IEnumerable<PhysicsShape.Contact> Filter(this NativeArray<PhysicsShape.Contact> contacts,
            ContactNormalVectorFilterFunction filterFunction, PhysicsShape shapeContext = default) =>
            new FilteredContacts(contacts, filterFunction, shapeContext);

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
        private struct FilteredContacts : IEnumerable<PhysicsShape.Contact>, IEnumerator<PhysicsShape.Contact>
        {
            public FilteredContacts(NativeArray<PhysicsShape.Contact> contacts, ContactFilterFunction filterFunction,
                PhysicsShape shapeContext)
            {
                m_Contacts = contacts;
                m_ShapeContext = shapeContext;
                _normalFilterFunction = filterFunction;
                _normalVectorFilterFunction = null;
                m_Index = -1;
            }
            
            public FilteredContacts(NativeArray<PhysicsShape.Contact> contacts,
                ContactNormalVectorFilterFunction filterFunction,
                PhysicsShape shapeContext)
            {
                m_Contacts = contacts;
                m_ShapeContext = shapeContext;
                _normalFilterFunction = null;
                _normalVectorFilterFunction = filterFunction;
                m_Index = -1;
            }

            private readonly NativeArray<PhysicsShape.Contact> m_Contacts;
            private readonly PhysicsShape m_ShapeContext;
            private readonly ContactFilterFunction _normalFilterFunction;
            private readonly ContactNormalVectorFilterFunction _normalVectorFilterFunction;
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
                    if (_normalFilterFunction(ref contact, m_ShapeContext))
                        return true;
                }

                return false;
            }

            void IEnumerator.Reset() => m_Index = -1;

            public IEnumerator<PhysicsShape.Contact> GetEnumerator() =>
                new FilteredContacts(m_Contacts, _normalFilterFunction, m_ShapeContext);

            IEnumerator IEnumerable.GetEnumerator() =>
                new FilteredContacts(m_Contacts, _normalFilterFunction, m_ShapeContext);

            // Iterator does not own the buffer, nothing to dispose.
            public readonly void Dispose()
            {
            }

            #endregion
        }
    }
}