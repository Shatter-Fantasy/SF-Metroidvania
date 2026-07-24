
using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
	using SFEditor.Graphs.Nodes;
	using SF.DialogueModule.Nodes;
	using SF.Graphs.Nodes;

	[System.Serializable]
    [UseWithContext(typeof(ConversationContextNode))] 
	[UseWithGraph(typeof(DialogueGraph))]
    class ConversationEntryBlockNode : BlockNode, IDialogueNode,INodeConvertor
    {
	    public string ExecutionPortName { get; } = "Conversation Entry";
	    public string SpeakerOptionsName { get; } = "Speaker";
	    protected override void OnDefineOptions(IOptionDefinitionContext  context)
	    {		    
		    context.AddOption<string>(SpeakerOptionsName);
		    context.AddOption<string>(ExecutionPortName).AsTextArea().Build();
	    }

	    public SFRuntimeNode ConvertToRuntimeNode()
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