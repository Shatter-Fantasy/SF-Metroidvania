using Unity.GraphToolkit.Editor;

namespace SFEditor.DialogueModule
{
	using SF.Graphs.Nodes.Editor;

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