using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule.Nodes
{
    [System.Serializable]
    public class ComparisonRuntimeNode : RuntimeNode, IComparisonNode<int>
    {
        [field:SerializeField] public int ValueToCheck { get; set; }
        [field:SerializeField] public int ComparisonValue { get; set; }

        [SerializeReference]
        public IRuntimeNode ExecutionNode;
        public ComparisonRuntimeNode(int valueToCheck, int comparisonValue, IRuntimeNode executionNode = null)
        {
            ValueToCheck = valueToCheck;
            ComparisonValue = comparisonValue;
            ExecutionNode = executionNode;
        }

        public bool Compare()
        {
            return ValueToCheck == ComparisonValue;
        }

        public override void TraverseNode(in List<RuntimeNode> branchNodes)
        {
            ExecutionNode?.TraverseNode(branchNodes);
        }

        public override void ProcessNode()
        {
            ExecutionNode?.ProcessNode();
        }
    }
}
