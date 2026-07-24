using SF.Graphs.Nodes;
using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
    using SF.DialogueModule.Nodes;
    using SFEditor.Graphs.Nodes;
    using SF.DialogueModule;

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

        public SFRuntimeNode ConvertToRuntimeNode()
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