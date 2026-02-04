using Unity.GraphToolkit.Editor;
namespace SFEditor.Dialogue.Graphs
{
	
    [UseWithContext(typeof(ConversationContextNode))] [System.Serializable]
    [UseWithGraph(typeof(DialogueGraph))]
    class ConversationChoiceBlockNode : BlockNode
    {
	    public const string ChoiceOneName = "Choice One"; 
	    public const string ChoiceTwoName = "Choice Two"; 
	    
	    protected override void OnDefineOptions(IOptionDefinitionContext  context)
	    {
		    context.AddOption<string>(ChoiceOneName);
		    context.AddOption<string>(ChoiceTwoName);
	    }

	    protected override void OnDefinePorts(IPortDefinitionContext context)
	    {
		    context.AddOutputPort<IDialogueNode>(ChoiceOneName);
		    context.AddOutputPort<IDialogueNode>(ChoiceTwoName);
	    }
    }
}
