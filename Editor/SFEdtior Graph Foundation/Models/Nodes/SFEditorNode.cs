using Unity.GraphToolkit.Editor;

namespace SFEditor.Graphs.Nodes
{
    using SF.Graphs.Nodes;

    /// <summary>
    /// This interface is used to allow a List of editor based SF nodes to contain both <see cref="Node"/> and <see cref="ContextNode"/>.
    /// By having <see cref="SFEditorNode"/> and <see cref="SFEditorContextNode"/> both implement it convariant casting can be used to simplify some node proccessing from editor to runtime nodes.
    /// </summary>
    public interface ISFEditorNode : ISFNode, INode
    {
        public abstract string ExecutionPortName { get; }
    }

    /// <summary>
    /// The base class for any SF Editor based nodes.
    /// This can be for <see cref="ContextNode"/> or <see cref="Node"/>
    /// </summary>
    [System.Serializable]
    public abstract class SFEditorNode : Node, ISFEditorNode
    {
        public abstract string ExecutionPortName { get; }
    }

    /// <summary>
    /// The base class for any SF Editor based nodes.
    /// This can be for <see cref="ContextNode"/> or <see cref="Node"/>
    /// </summary>
    [System.Serializable]
    public abstract class SFEditorContextNode : ContextNode, ISFEditorNode
    {
        public abstract string ExecutionPortName { get; }
    }
}