using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;

namespace SFEditor.Graphs.Nodes
{
    using SF.Graphs.Nodes;
    using SFEditor.Graphs.Nodes;

   public class SFNodeProcessor<TGraphBase, IGraphNode>
        where TGraphBase : SFGraphBase
        where IGraphNode : class, INode, ISFEditorNode // doing class, INode, ISFNode allows processing both normal Nodes and ContextNodes.
    {
        protected TGraphBase _graph;
        public List<IGraphNode> GraphNodes = new();
        public List<SFRuntimeNode> RuntimeNodes = new();

        private SFEditorStartNode _startNode;

        public SFNodeProcessor(TGraphBase graphBase, bool processNodesInstantly = true)
        {
            _graph = graphBase;

            if (processNodesInstantly)
                StartNodeProcessing();
        }

        public virtual void StartNodeProcessing()
        {
            // Unity's built in nodes method returns the nodes as a IEnumerable<INode> so we convert it to whatever type we plan to use.
            // Note the non-generic SFGraphBase doesn't have the built in GetNodesAsDeclaredType so we have to manually do this here.
            GraphNodes = _graph?.GetNodes().ToList().ConvertAll(xnode => (IGraphNode)xnode);

            // The starting node tells each graph where they can possibly start.
            _startNode = GraphNodes?.OfType<SFEditorStartNode>().FirstOrDefault();

            if(_startNode == null)
                return;

            ProcessNodesToConversations(_startNode as IGraphNode);
        }

        protected virtual void ProcessNodesToConversations(in IGraphNode nodeModel)
        {
            if (nodeModel is IContextNodeConvertor contextNodeConvertor)
            {
                RuntimeNodes.Add(contextNodeConvertor.ConvertToRuntimeNode());
                //_runtimeNodes.AddRange(contextNodeConvertor.ConvertToRuntimeNodes(_dialogueConversation));
            }

            var nextNode = GetNextNode(nodeModel);

            if (nextNode != null)
            {
                ProcessNodesToConversations(nextNode);
            }
        }

        public static IGraphNode GetNextNode<TGraphNode>(TGraphNode currentNode) where TGraphNode : class, IGraphNode
        {
            var outputPort = currentNode.GetOutputPortByName(currentNode.ExecutionPortName);
            var nextNodePort = outputPort.FirstConnectedPort;
            var nextNode = nextNodePort?.GetNode() as TGraphNode;
            return nextNode;
        }
    }
}