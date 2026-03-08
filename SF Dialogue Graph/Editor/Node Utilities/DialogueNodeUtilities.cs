using Unity.GraphToolkit.Editor;

namespace SFEditor.DialogueModule
{
	using SF.DialogueModule.Nodes;
	using Dialogue.Graphs;
	using SFEditor.Nodes;
    public static class DialogueNodeUtilities
    {
		
		public static IDialogueNode GetNextNode<T>(T currentNode) where T : IDialogueNode
	    {
		    var outputPort = currentNode.GetOutputPortByName(currentNode.ExecutionPortName);
		    
#if UNITY_6000_4_OR_NEWER
		    var nextNodePort = outputPort.FirstConnectedPort;
#else
		    var nextNodePort = outputPort.firstConnectedPort;
#endif
		    
		    var nextNode = nextNodePort?.GetNode() as IDialogueNode;
		    return nextNode;
	    }
	    
	   
    }
}
