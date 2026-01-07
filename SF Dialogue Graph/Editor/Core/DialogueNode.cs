using Unity.GraphToolkit.Editor;

namespace SFEditor.Dialogue.Graphs
{
    interface IDialogueNode : INode
    {
        public string ExecutionPortName { get; }
    }

    /// <summary>
    /// The base class containing runtime data for dialogue nodes.
    /// </summary>
    [System.Serializable]
    public abstract class DialogueNode : Node, IDialogueNode
    {
        public abstract string ExecutionPortName { get; }
    }
}
