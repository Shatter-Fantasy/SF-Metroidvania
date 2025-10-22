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
        /// <returns>An enumerable set of contacts filtered by the filter function.</returns>
        public static IEnumerable<PhysicsShape.Contact> Filter(
            this NativeArray<PhysicsShape.Contact> contacts,
            ContactNormalFilterFunction filterFunction,
            PhysicsShape shapeContext = default,
            float normalizedYValue = 0f, 
            FilterMathOperator filterMathOperator = FilterMathOperator.Equal) =>
             new FilteredContacts(contacts, filterFunction, shapeContext, normalizedYValue, filterMathOperator);
        

        private struct FilteredContacts : IEnumerable<PhysicsShape.Contact>, IEnumerator<PhysicsShape.Contact>
        {
            private readonly NativeArray<PhysicsShape.Contact> _contacts;
            private readonly PhysicsShape _shapeContext;
            private readonly ContactNormalFilterFunction _normalFilterFunction;
            private float _normalizedValue;
            private FilterMathOperator _filterMathOperator;
            private int _index;
            
            public FilteredContacts(NativeArray<PhysicsShape.Contact> contacts,
                ContactNormalFilterFunction filterFunction,
                PhysicsShape shapeContext,
                float normalizedValue = 0f, 
                FilterMathOperator filterMathOperator = FilterMathOperator.Equal
                )
            {
                _contacts = contacts;
                _shapeContext = shapeContext;
                _normalFilterFunction = filterFunction;
                _normalizedValue = normalizedValue;
                _filterMathOperator = filterMathOperator;
                _index = -1;
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
                new FilteredContacts(_contacts, _normalFilterFunction, _shapeContext);

            IEnumerator IEnumerable.GetEnumerator() =>
                new FilteredContacts(_contacts, _normalFilterFunction, _shapeContext);

            // Iterator does not own the buffer, nothing to dispose.
            public readonly void Dispose()
            {
            }

            #endregion
        }

    }
}