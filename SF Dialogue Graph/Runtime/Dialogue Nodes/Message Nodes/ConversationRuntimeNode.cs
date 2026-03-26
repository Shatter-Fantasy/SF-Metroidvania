using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule.Nodes
{
    [System.Serializable]
    public class ConversationRuntimeNode : RuntimeNode
    {
        /// <summary>
        /// Node to process after going through all the block nodes in this context.
        /// </summary>
        [SerializeReference]
        public IRuntimeNode ExecutionNode;
        
        [SerializeReference]
        public List<IRuntimeNode> RuntimeNodes = new();
        
        public ConversationRuntimeNode(List<IRuntimeNode> runtimeNodes, IRuntimeNode executionNode = null)
        {
            if(runtimeNodes != null)
                RuntimeNodes = runtimeNodes;
            
            if (executionNode != null)
                ExecutionNode = executionNode;
        }

        public override void TraverseNode(in List<RuntimeNode> branchNodes)
        {
            foreach (var node in RuntimeNodes)
            {
                node.Conversation = Conversation;
                node.TraverseNode(branchNodes);
            }
            
            ExecutionNode?.TraverseNode(branchNodes);
        }

        public override void ProcessNode()
        {
            foreach (var node in RuntimeNodes)
            {
                node.ProcessNode();
            }
        
            ExecutionNode?.ProcessNode();
        }
    }
}
