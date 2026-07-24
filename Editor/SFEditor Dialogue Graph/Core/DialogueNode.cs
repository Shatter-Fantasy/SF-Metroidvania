using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
    using SFEditor.Graphs.Nodes;
    using SF.Graphs.Nodes;

    public interface IDialogueNode : INode, ISFNode
    {
        public string ExecutionPortName { get; }
    }

    /// <summary>
    /// The base class containing runtime data for dialogue nodes.
    /// </summary>
    [System.Serializable]
    public abstract class DialogueNode : SFEditorNode, IDialogueNode
    {
    }
}