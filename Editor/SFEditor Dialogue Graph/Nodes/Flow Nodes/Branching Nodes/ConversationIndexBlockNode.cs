using SF.DialogueModule;
using Unity.GraphToolkit.Editor;
using SF.DialogueModule.Nodes;
using SFEditor.Nodes;

namespace SFEditor.Dialogue.Graphs
{
    [System.Serializable]
    [UseWithContext(typeof(BranchingContextNode), typeof(ConversationContextNode))]
    [UseWithGraph(typeof(DialogueGraph))]
    public class ConversationIndexBlockNode : BlockNode, INodeConvertor
    {
        public DialogueConversation Conversation;
        
        public ConversationIndexNodeType ConversationIndexNodeType;
        
        public const string NodeTypeOptionName = "Node Operation";
        public const string ConversationIndexOptionName = "Conversation Index";

        private int _conversationIndex;
        protected override void OnDefineOptions(IOptionDefinitionContext  context)
        {
            context.AddOption<ConversationIndexNodeType>(NodeTypeOptionName);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            GetNodeOptionByName(NodeTypeOptionName).TryGetValue(out ConversationIndexNodeType);
           
            
            if(ConversationIndexNodeType == ConversationIndexNodeType.Set)
                context.AddInputPort<int>(ConversationIndexOptionName);
        }

        public IRuntimeNode ConvertToRuntimeNode()
        {
            GetNodeOptionByName(NodeTypeOptionName).TryGetValue(out ConversationIndexNodeType);

            if (ConversationIndexNodeType == ConversationIndexNodeType.Set)
            {
                GetInputPortByName(ConversationIndexOptionName).TryGetValue(out _conversationIndex);
            }

            return new ConversationIndexRuntimeNode(ConversationIndexNodeType, _conversationIndex);
        }
    }

}