using System.Collections.Generic;
using SF.DialogueModule;
using Unity.GraphToolkit.Editor;
using SFEditor.Nodes;
using SF.DialogueModule.Nodes;
using UnityEngine;

namespace SFEditor.Dialogue.Graphs
{
    [System.Serializable]
    [UseWithGraph(typeof(DialogueGraph))]
    public class BranchingContextNode : ContextNode, IContextNodeConvertor, IDialogueNode
    {
        [SerializeReference]
        public List<IComparisonNode> ComparisonNodes = new();
        
        public string ExecutionPortName { get; } = "Comparisons Failed";
        public const string ValuePort = "Value";
        
        public List<IRuntimeNode> RuntimeNodes = new ();
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<string>("Input Node");
            context.AddInputPort<float>(ValuePort);
		    
            context.AddOutputPort<string>(ExecutionPortName);
        }

        public IRuntimeNode ConvertToRuntimeNode()
        {
            RuntimeNodes = ConvertToRuntimeNodes();

            return new BranchingRuntimeNode(RuntimeNodes);
        }

        public List<IRuntimeNode> ConvertToRuntimeNodes(DialogueConversation dialogueConversation = null)
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
