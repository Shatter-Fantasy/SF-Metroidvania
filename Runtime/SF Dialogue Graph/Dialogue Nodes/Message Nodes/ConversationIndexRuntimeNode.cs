using System.Collections.Generic;

namespace SF.DialogueModule.Nodes
{
    using SF.Graphs.Nodes;

    public enum ConversationIndexNodeType
    {
        Get,
        Set
    }
    
    [System.Serializable]
    public class ConversationIndexRuntimeNode : ConversationRuntimeNode
    {
        public ConversationIndexNodeType IndexOperation;
        public int ConversationIndex;

        public ConversationIndexRuntimeNode(ConversationIndexNodeType nodeType, int conversationIndex = -1)
        {
            IndexOperation = nodeType;
            ConversationIndex = conversationIndex;
        }

        public override void TraverseNode(in List<SFRuntimeNode> branchNodes)
        {
            branchNodes.Add(this);
        }

        public override void ProcessNode()
        {
            if (Conversation == null)
                return;
            
            // TODO: Make an end process C# Action for retrieving the value returned during a Get operation.
            if(IndexOperation == ConversationIndexNodeType.Set)
                Conversation.ConversationIndex = ConversationIndex;
            else
            {
                ConversationIndex = Conversation.ConversationIndex;
            }
        }

    }
}