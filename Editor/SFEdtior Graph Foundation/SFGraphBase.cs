using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace SFEditor.Graphs
{
    using SFEditor.Graphs.Nodes;

    [System.Serializable]
    public abstract class SFGraphBase : Graph
    {
        /// <summary>
        /// If the graph has changed we need to update it during the next graph importer.
        /// </summary>
        public bool HasGraphChanged;

        public bool HasInitialized = false;

        /// <summary>
        /// The current node to use as a starting point for proccessing nodes.
        /// </summary>
        public SFEditorStartNode StartingGraphNode;

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            HasGraphChanged = true;
        }
    }


    /// <summary>
    /// Base class for any <see cref="SFGraphBase"/> with a defined type for nodes allowed in it.
    /// Most cases this can just be the base <see cref="SFEditorNode"/> type for allowing the most flexibility for editor graph nodes.
    /// </summary>
    /// <typeparam name="IGraphNode">The base interface for any of the nodes that will be used in the implemented Graph.</typeparam>
    [System.Serializable]
    public abstract class SFGraphBase<IGraphNode> : SFGraphBase where IGraphNode : SFEditorNode
    {
        public List<IGraphNode> DeclaredTypedNodes = new();

        /// <summary>
        /// Gets the nodes in the calling <see cref="SFGraphBase{IGraphNode}"/> as a list of the declared <see cref="IGraphNode"/> type.
        /// This also updates the current <see cref="DeclaredTypedNodes"/> list just in case a new node was added to the graph, but not had the declared type of it cached yet.
        /// </summary>
        /// <returns></returns>
        public virtual List<IGraphNode> GetNodesAsDeclaredType()
        {
            DeclaredTypedNodes = GetNodes().ToList().ConvertAll(xnode => (IGraphNode)xnode);
            return DeclaredTypedNodes;
        }
    }
}