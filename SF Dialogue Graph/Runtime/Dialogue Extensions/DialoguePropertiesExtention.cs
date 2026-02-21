using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule
{
    public class DialoguePropertiesExtension : DialogueExtensionBase
    {
        public List<ConversationPropertyPair> ConversationProperties = new();
        
        public override void ControlFlow()
        {
            if (RuntimeGraph != null && ConversationProperties?.Count > 0)
            {
                foreach (var conversationProperty in ConversationProperties)
                {
                    RuntimeGraph.ClonedConversation.AddProperty(conversationProperty.PropertyName, conversationProperty.Value);
                }
            }
        }
    }
    
    public class ConversationPropertyPair
    {
        
        public string PropertyName;
        /// <summary>
        /// The object that is the value in the property pair.
        /// </summary>
        public object Value;

        public Type ValueType;

        public ConversationPropertyPair(Component value, string name)
        {
            Value = value;
            ValueType = Value.GetType();
            PropertyName = name;
        }
        
        public ConversationPropertyPair(object value, string name)
        {
            Value = value;
            ValueType = Value.GetType();
            PropertyName = name;
        }

        /// <summary>
        /// Casts the <see cref="Value"/> as the requested type of T.
        ///
        /// Returns null if the <see cref="Value"/> was not of the requested <see cref="TObject"/> type or
        /// as type that can be boxed from it.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public TObject GetValueObject<TObject>()
        {
           return (TObject)Value;
        }
    }
}
