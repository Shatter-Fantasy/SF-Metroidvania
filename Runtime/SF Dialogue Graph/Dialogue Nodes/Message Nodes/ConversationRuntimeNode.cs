using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule.Nodes
{
    using SF.Graphs.Nodes;


    [System.Serializable]
    public class ConversationRuntimeNode : SFRuntimeNode
    {
        /// <summary>
        /// Node to process after going through all the block nodes in this context.
        /// </summary>
        [SerializeReference]
        public SFRuntimeNode ExecutionNode;

        public DialogueConversation Conversation { get;set; }

        [SerializeReference]
        public List<SFRuntimeNode> RuntimeNodes = new();

        /// <summary>
        /// Empty constructor so inheriting classes don't need to implement a paramarless constructor.
        /// </summary>
        public ConversationRuntimeNode(){}
        public ConversationRuntimeNode(List<SFRuntimeNode> runtimeNodes, SFRuntimeNode executionNode = null)
        {
            if(runtimeNodes != null)
                RuntimeNodes = runtimeNodes;
            
            if (executionNode != null)
                ExecutionNode = executionNode;
        }

        public override void TraverseNode(in List<SFRuntimeNode> branchNodes)
        {
            
            foreach (var node in RuntimeNodes)
            {
                if(node is ConversationRuntimeNode conversationRuntimeNode)
                    conversationRuntimeNode.Conversation = Conversation;
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