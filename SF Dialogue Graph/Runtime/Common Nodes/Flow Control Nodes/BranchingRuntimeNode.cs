using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule.Nodes
{
    [System.Serializable]
    public class BranchingRuntimeNode : RuntimeNode
    {
        /// <summary>
        /// This is the node to go through if none of the comparison
        /// checks nodes passed the validation checks.
        /// </summary>
        [SerializeReference] public IRuntimeNode FallbackNode; 
        
        [SerializeReference]
        public List<IRuntimeNode> ComparisonNodes = new();

        public BranchingRuntimeNode(List<IRuntimeNode> comparisonNodes)
        {
            if (comparisonNodes != null)
                ComparisonNodes = comparisonNodes;
        }

        public override void TraverseNode(in List<RuntimeNode> branchNodes)
        {
            foreach (var node in ComparisonNodes)
            {
                if (node is not IComparisonNode comparisonNode || !comparisonNode.Compare()) 
                    continue;
                
                node.TraverseNode(branchNodes);
                
                /* We return after processing the node branch to prevent any
                 * comparison nodes in the ComparisonNodes list to be process. */
                return;
            }
            // No node had a valid comparison so we do the default node.
            
            FallbackNode?.TraverseNode(branchNodes);

        }

        public override void ProcessNode()
        {
            foreach (var node in ComparisonNodes)
            {
                if (node is not IComparisonNode comparisonNode || !comparisonNode.Compare()) 
                    continue;
                
                node.ProcessNode();
                
                /* We return after processing the node branch to prevent any 
                 * comparison nodes in the ComparisonNodes list to be process. */
                return;
            }
            // No node had a valid comparison so we do the default node.
            
            FallbackNode?.ProcessNode();
        }
    }
}
