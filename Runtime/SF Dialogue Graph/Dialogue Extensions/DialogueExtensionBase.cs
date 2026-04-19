using System;
using SF.DialogueModule.Nodes;
using UnityEngine;

namespace SF.DialogueModule
{
    /// <summary>
    /// Controls when the Conversation extension injects custom logic into the conversation flow.
    /// </summary>
    public enum ConversationStage
    {
        Start,
        Completed,
        DialogueNode
    }
    
    /// <summary>
    /// Base class to help allow easily created custom code for any type of Dialogue managers.
    /// </summary>
    public abstract class DialogueExtensionBase : MonoBehaviour
    {
        /// <summary>
        /// The runtime graph the extension is being called from.
        /// </summary>
        [NonSerialized] public DialogueRuntimeGraph RuntimeGraph;
        
        public ConversationStage Stage;
        
        /// <summary>
        /// If the ConversationStage is set to ConversaNode this id will be used to link to the conversa node. 
        /// </summary>
        public string NodeCallerID;

        /// <summary>
        /// Called from the <see cref="ConversationRunner"/> before it starts a conversation.
        /// </summary>
        /// <remarks>
        ///  Called Control flow because this is a form of a Pipeline Paradigm
        /// </remarks>
        public abstract void ControlFlow();
    }
}
