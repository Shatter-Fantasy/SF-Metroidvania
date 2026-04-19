using System;
using Unity.GraphToolkit.Editor;

using SF.DialogueModule;

namespace SFEditor.Dialogue.Graphs
{
    
    [Serializable]
    public class StartDialogueNode : DialogueNode, IDialogueNode
    {
        public override string ExecutionPortName { get; } = "Dialogue Start";
        public const string DialogueDBName = "Dialogue Database";
        public const string LocalizedDialogueTableName = "Localized Dialogue Table";
        public const string ConversationIDName = "Conversation ID";
	    
        protected override void OnDefineOptions(IOptionDefinitionContext  context)
        {
            context.AddOption<DialogueDatabase>(DialogueDBName);
            context.AddOption<int>(ConversationIDName);
        }
	    
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<string>(ExecutionPortName).Build();
        }
    }
    
    [Serializable]
    public class EndNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<INode>("Input Port");
        }
    }
}

