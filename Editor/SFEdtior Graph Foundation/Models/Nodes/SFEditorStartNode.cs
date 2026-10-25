using SFEditor.Graphs;

namespace SF.Graphs.Nodes.Editor
{
    /// <summary>
    /// Used to declare a specific node for a start of a <see cref="SFGraphBase"/>
    /// </summary>
    [System.Serializable]
    public class SFEditorStartNode : SFEditorNode
    {
        public override string ExecutionPortName { get; } = "Start Node";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort(ExecutionPortName).Build();
        }
    }
}