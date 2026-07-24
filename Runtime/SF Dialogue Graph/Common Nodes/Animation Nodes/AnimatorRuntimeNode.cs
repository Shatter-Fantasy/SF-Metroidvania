using System.Collections.Generic;
using UnityEngine;

namespace SF.Graphs.Nodes
{
    /// <summary>
    /// A runtime node used to play and control animations on the passed in animator.
    /// </summary>
    [System.Serializable]
    public class AnimatorRuntimeNode : SFRuntimeNode
    {
        /// <summary>
        /// The animator to set the animation parameters state of.
        /// </summary>
        public Animator Animator;

        /// <summary>
        ///  The animation parameter to update. 
        /// </summary>
        public string AnimationParameterName;

        public override void TraverseNode(in List<SFRuntimeNode> branchNodes)
        {
            branchNodes.Add(this);
        }

        public override void ProcessNode()
        {
            // TODO: This should be hooked up to whatever audio manager we want to use.
            // Most likely FMOD.
            if (Animator == null || string.IsNullOrEmpty(AnimationParameterName))
                return;
            
            Animator.SetTrigger(AnimationParameterName);
        }
    }
}