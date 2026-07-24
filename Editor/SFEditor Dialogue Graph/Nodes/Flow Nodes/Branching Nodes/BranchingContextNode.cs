using System.Collections.Generic;
using UnityEngine;

namespace SFEditor.Graphs.Nodes
{
    using SF.DialogueModule;
    using SF.DialogueModule.Nodes;
    using SF.Graphs.Nodes;

    [System.Serializable]
    public class BranchingContextNode : SFEditorContextNode, INodeConvertor
    {
        [SerializeReference]
        public List<IComparisonNode> ComparisonNodes = new();
        
        public override string ExecutionPortName { get; } = "Comparisons Failed";
        public const string ValuePort = "Value";
        
        public List<SFRuntimeNode> RuntimeNodes = new ();
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<string>("Input Node");
            context.AddInputPort<float>(ValuePort);
		    
            context.AddOutputPort<string>(ExecutionPortName);
        }

        public SFRuntimeNode ConvertToRuntimeNode()
        {
            RuntimeNodes = ConvertToRuntimeNodes();

            return new BranchingRuntimeNode(RuntimeNodes);
        }

        public List<SFRuntimeNode> ConvertToRuntimeNodes(DialogueConversation dialogueConversation = null)
        {
#if UNITY_6000_4_OR_NEWER
			for(int i = 0; i < BlockCount; i++) 
#else
            for (int i = 0; i < blockCount; i++)
#endif
            {
                var comparisonNode = GetBlock(i);

                if (comparisonNode is not INodeConvertor convertor)
                    return null;
                
                var convertedNode = convertor.ConvertToRuntimeNode();

                if (convertedNode == null)
                    continue;
                
                RuntimeNodes.Add(convertedNode);
            }
            
            return RuntimeNodes;
        }
    }
}