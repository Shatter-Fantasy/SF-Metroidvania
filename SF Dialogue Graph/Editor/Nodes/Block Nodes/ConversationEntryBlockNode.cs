using SFEditor.Nodes;
using SF.DialogueModule.Nodes;
using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
	[System.Serializable]
    [UseWithContext(typeof(ConversationContextNode))] 
	[UseWithGraph(typeof(DialogueGraph))]
    class ConversationEntryBlockNode : BlockNode, IDialogueNode,INodeConvertor
    {
	    public string ExecutionPortName { get; } = "Conversation Entry";
	    public string SpeakerOptionsName { get; } = "Speaker";
	    public const string AnimationParameterOptionsName = "Animation Parameter";
	    protected override void OnDefineOptions(IOptionDefinitionContext  context)
	    {		    
		    context.AddOption<string>(SpeakerOptionsName);
		    context.AddOption<string>(ExecutionPortName);
		    context.AddOption<string>(AnimationParameterOptionsName);
	    }

	    public IRuntimeNode ConvertToRuntimeNode()
	    {
		    GetNodeOptionByName(ExecutionPortName).TryGetValue(out string text);
		    GetNodeOptionByName(SpeakerOptionsName).TryGetValue(out string speakerName);

		    return new ConversationEntryRuntimeNode()
		    {
			    Text = text,
			    SpeakerName = speakerName
		    };
	    }
    }
}
