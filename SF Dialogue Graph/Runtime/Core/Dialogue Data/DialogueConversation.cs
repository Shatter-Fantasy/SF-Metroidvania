using System;
using System.Collections.Generic;
using System.Linq;
using SF.DialogueModule.Nodes;
using UnityEngine;

namespace SF.DialogueModule
{
    
    /// <summary>
    /// Data container for an entire conversation of a dialogue sequence.
    /// Including dialogue entries, actor data, and event details related to the dialogue.
    /// </summary>
    [CreateAssetMenu(menuName = "SF/Dialogue/Dialogue Conversation", fileName = "SF Conversation")]
    public class DialogueConversation : ScriptableObject
    {
        /// <summary>
        /// The name of this conversation.
        /// </summary>
        public string Name;
        /// <summary>
        /// The unique identifier for a <see cref="DialogueConversation"/>.
        /// </summary>
        public int GUID;
        
        //public List<DialogueEntry> DialogueEntries = new();

        [SerializeReference]
        public List<IRuntimeNode> Nodes = new();
        /// <summary>
        /// This is the runtime graph for conversations nodes created from the Dialogue Graph Importer.
        /// 
        /// </summary>
        public DialogueRuntimeGraph RuntimeGraph;
        
        /// <summary>
        /// Index of the current active instance of the conversation. -1 if not the active conversation.
        /// </summary>
        public int ConversationIndex = 0;

        /// <summary>
        /// The index of the current Dialogue Entry being shown for the active conversation if in a conversation.
        /// </summary>
        /// <remarks>
        /// This is set to zero when a conversation starts from <see cref="DialogueManager.TriggerConversation"/>
        /// </remarks>
        [NonSerialized] public int DialogueEntryIndex = -1;
        
        public List<ConversationPropertyPair> Properties = new();
        /// <summary>
        ///  Blank constructor for allowing new conversations when none in the database is found with a matching guid.
        /// </summary>
        public DialogueConversation() { }
        
        public DialogueConversation(List<IRuntimeNode> nodes)
        {
            RuntimeGraph = new DialogueRuntimeGraph(this,nodes);
            Nodes = nodes;

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Conversation = this;
            }
        }

        public DialogueConversation Clone(DialogueConversation conversation)
        {
            DialogueConversation clonedConversation =  Instantiate(conversation);
            
            if (clonedConversation == null 
                || conversation.RuntimeGraph == null)
                return clonedConversation;
            
            clonedConversation.Nodes = new List<IRuntimeNode>(conversation.RuntimeGraph.Nodes);
            
            return clonedConversation;
        }
        
        /// <summary>
        /// Set a property inside of the dialogue conversation.
        /// </summary>
        /// <param name="propertyPair"></param>
        public void AddProperty(ConversationPropertyPair propertyPair)
        {
            Properties.Add(propertyPair);
        }

        /// <summary>
        /// Set a property inside of the dialogue conversation.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="TComponent"></typeparam>
        public void AddProperty<TValue>(string propertyName, TValue value)
        {
            Properties.Add(new ConversationPropertyPair(value,propertyName));
        }
        
       
        
        
        public T GetProperty<T>(string propertyName, T value)
        {
          var property = Properties.FirstOrDefault(prop => prop.PropertyName == propertyName);
          return property.GetValueObject<T>();
        }

        /// <summary>
        /// Used to update the graph nodes without resetting the reference in scriptable object assets.
        /// </summary>
        public void UpdateNodes(List<IRuntimeNode> updatedNodes)
        {
            for (int i = 0; i < updatedNodes.Count; i++)
            {
                if (i > RuntimeGraph.Nodes.Count - 1)
                {
                    RuntimeGraph.Nodes.Add(updatedNodes[i]);
                }
                else
                {
                    if(updatedNodes[i] is RuntimeNode runtimeNode)
                        RuntimeGraph.Nodes[i] = runtimeNode.ShallowCopy();
                }
            }
        }
        
        public void StopConversation()
        {
            DialogueEntryIndex = -1;
        }
    }
}
