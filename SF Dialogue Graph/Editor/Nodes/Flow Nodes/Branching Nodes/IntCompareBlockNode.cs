using Unity.GraphToolkit.Editor;
using SF.DialogueModule.Nodes;
using SFEditor.Nodes;

namespace SFEditor.Dialogue.Graphs
{

    public abstract class ComparisonBlockNode : BlockNode, IComparisonNode
    {
        public string ExecutionPortName { get; } = "Comparison Passed";
        
        /// <summary>
        /// The node to go to next if <see cref="Compare"/> returns true.
        /// </summary>
        protected INode _executionNode;
        
        public INode GetExecutionNode()
        {
#if UNITY_6000_4_OR_NEWER
            return _executionNode = GetOutputPortByName(ExecutionPortName).FirstConnectedPort.GetNode();
#else
            return _executionNode = GetOutputPortByName(ExecutionPortName).firstConnectedPort.GetNode();
#endif  
          
        }

        public abstract bool Compare();
    }
    
    [System.Serializable]
    [UseWithContext(typeof(BranchingContextNode))]
    [UseWithGraph(typeof(DialogueGraph))]
    public class IntCompareBlockNode : ComparisonBlockNode, INodeConvertor
    {
        public ComparisonNodeType ComparisonNodeType;
        
        /// <summary>
        /// The value you want to use a the base to check.
        /// This is set by the <see cref="BranchingContextNode"/>
        /// </summary>
        public int ValueToCheck;
        /// <summary>
        /// The value to check against.
        /// </summary>
        public int ComparisonValue;
        
        public string ComparisonValueOptionsName { get; } = "Comparison Value";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort<string>(ExecutionPortName);
        }
        
        protected override void OnDefineOptions(IOptionDefinitionContext  context)
        {		    
            context.AddOption<int>(ComparisonValueOptionsName);
        }
        
        public override bool Compare()
        {
            return ValueToCheck == ComparisonValue;
        }
        
        public IRuntimeNode ConvertToRuntimeNode()
        {
            GetInputPortByName(BranchingContextNode.ValuePort)
                .TryGetValue(out ValueToCheck);
            
            GetNodeOptionByName(ComparisonValueOptionsName).TryGetValue(out ComparisonValue);
            GetExecutionNode();

            if (_executionNode is INodeConvertor nodeConvertor)
            {
                return new ComparisonRuntimeNode
                (
                    ValueToCheck, 
                    ComparisonValue, 
                    nodeConvertor.ConvertToRuntimeNode()
                );
            }

            return new ComparisonRuntimeNode(ValueToCheck, ComparisonValue);
        }
    }
}
