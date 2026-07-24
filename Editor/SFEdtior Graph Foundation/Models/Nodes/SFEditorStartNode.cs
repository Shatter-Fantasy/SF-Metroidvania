namespace SFEditor.Graphs.Nodes
{
    using SF.Graphs.Nodes;

    /// <summary>
    /// Used to declare a specific node for a start of a <see cref="SFGraphBase"/>
    /// </summary
    [System.Serializable]
    public class SFEditorStartNode : SFEditorNode, ISFNode
    {
        public override string ExecutionPortName { get; } = "Start Node";
    }
}