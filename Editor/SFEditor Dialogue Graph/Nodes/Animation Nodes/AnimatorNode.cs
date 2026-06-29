using SF.DialogueModule.Nodes;
using SFEditor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace SFEditor.Dialogue.Graphs
{
    /// <summary>
    /// The type of animation value an <see cref="AnimatorNode"/> is setting.
    /// </summary>
    public enum AnimatorNodeType
    {
        SetTrigger, // Animator.SetTrigger
        SetBool,  // Animator.SetBool 
        SetFloat, // Animator.SetFloat
        SetInteger // Animator.SetInteger
    }
    
    [System.Serializable]
    [UseWithContext(typeof(ConversationContextNode))]
    [UseWithGraph(typeof(DialogueGraph))]
    public class AnimatorNode : BlockNode, INodeConvertor
    {
        public const string AnimatorOptionsName = "Animator";
        public const string AnimatorNodeTypeOptionsName = "Animator Node Type";
        public const string AnimationParameterOptionsName = "Animation Parameter";
        
        public const string BoolOptionsName = "Animation Bool";
        public const string IntegerOptionsName = "Animation Integer";
        public const string FloatOptionsName = "Animation Float";
        
        [SerializeField] private AnimatorNodeType _animatorNodeType;

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {		    
            context.AddOption<AnimatorNodeType>(AnimatorNodeTypeOptionsName)
                .WithDefaultValue(_animatorNodeType)
                .Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<Animator>(AnimatorOptionsName);
            context.AddInputPort<string>(AnimationParameterOptionsName);
            
            var nodeTypeOption = GetNodeOptionByName(AnimatorNodeTypeOptionsName);
            
            if (nodeTypeOption != null)
            {
                nodeTypeOption.TryGetValue(out AnimatorNodeType nodeType);
                _animatorNodeType = nodeType;
                
                switch (_animatorNodeType)
                {
                    case AnimatorNodeType.SetTrigger:
                        break;
                    case AnimatorNodeType.SetBool:
                        context.AddInputPort<bool>(BoolOptionsName);
                        break;
                    case AnimatorNodeType.SetFloat:
                        context.AddInputPort<float>(FloatOptionsName);
                        break;
                    case AnimatorNodeType.SetInteger:
                        context.AddInputPort<int>(IntegerOptionsName);
                        break;
                }
            }
        }

        public IRuntimeNode ConvertToRuntimeNode()
        {
            GetNodeOptionByName(AnimatorNodeTypeOptionsName).TryGetValue(out AnimatorNodeType animatorNodeType);
            GetInputPortByName(AnimatorOptionsName).TryGetValue(out Animator animator);
            GetInputPortByName(AnimationParameterOptionsName).TryGetValue(out string animationParameter);
            
            return new AnimatorRuntimeNode();
        }
    }
}
