using Unity.GraphToolkit.Editor;

namespace SFEditor.DialogueModule
{
	using SFEditor.Graphs.Nodes;
	using Dialogue.Graphs;

    public static class DialogueNodeUtilities
    {
		public static ISFEditorNode GetNextNode<T>(T currentNode) where T : ISFEditorNode
	    {
		    var outputPort = currentNode.GetOutputPortByName(currentNode.ExecutionPortName);
		    var nextNodePort = outputPort.FirstConnectedPort;
		    
		    var nextNode = nextNodePort?.GetNode() as ISFEditorNode;
		    return nextNode;
	    }
    }
}