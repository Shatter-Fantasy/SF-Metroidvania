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
            
            branchNodes.Add(this);
            
            ExecutionNode?.TraverseNode(branchNodes);
        }

        public override async void ProcessNode()
        {
            foreach (var node in RuntimeNodes)
            {
                node.ProcessNode();
                DialogueManager.Instance.RuntimeGraph.IsPaused = node.ShouldPauseGraphProcessing;
                
                while (DialogueManager.Instance.RuntimeGraph.IsPaused)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
        
            ExecutionNode?.ProcessNode();
        }
    }
}
