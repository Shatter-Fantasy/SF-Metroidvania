using System.Collections.Generic;
using SF.DialogueModule;
using SF.DialogueModule.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace SFEditor.Nodes
{
    /// <summary>
    /// Describes how to convert an editor node to a runtime node for processing.
    /// </summary>
    public interface INodeConvertor
    {
        /// <summary>
        /// Tells how a graph importing class should convert editor nodes to runtime nodes.
        /// </summary>
        /// <returns>
        /// Returns the converted runtime node.
        /// </returns>
        public IRuntimeNode ConvertToRuntimeNode();
        
        // Maybe add a convert back to editor version from a runtime node.
    }
    
    /// <summary>
    /// Describes how to convert an editor node to a runtime node for processing.
    /// </summary>
    public interface IContextNodeConvertor : INodeConvertor
    {
        /// <summary>
        /// Tells how a graph importing class should convert editor nodes to runtime nodes.
        /// </summary>
        /// <returns>
        /// Returns the converted runtime node.
        /// </returns>
        public List<IRuntimeNode> ConvertToRuntimeNodes(DialogueConversation dialogueConversation);
        
        // Maybe add a convert back to editor version from a runtime node.
    }
}