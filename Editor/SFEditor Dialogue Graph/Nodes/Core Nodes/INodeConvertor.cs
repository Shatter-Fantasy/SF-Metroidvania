using System.Collections.Generic;

namespace SFEditor.Graphs.Nodes
{
    using SF.Graphs.Nodes;

    /// <summary>
    /// Describes how to convert an editor node to a runtime node for processing.
    /// </summary>
    public interface INodeConvertor
    {
        /// <summary>
        /// Tells how a graph importing class should convert editor nodes to runtime nodes.
        /// </summary>
        /// <returns>
        /// Returns the converted runtime node.
        /// </returns>
        public SFRuntimeNode ConvertToRuntimeNode();

        // Maybe add a convert back to editor version from a runtime node.
    }

    /// <summary>
    /// Describes how to convert an editor node to a runtime node for processing.
    /// </summary>
    public interface IContextNodeConvertor : INodeConvertor
    {
        /// <summary>
        /// Tells how a graph importing class should convert editor nodes to runtime nodes.
        /// </summary>
        /// <returns>
        /// Returns the converted runtime node.
        /// </returns>
        public List<SFRuntimeNode> ConvertToRuntimeNodes();

        // Maybe add a convert back to editor version from a runtime node.
    }
}