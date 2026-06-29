using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule.Nodes
{
	public interface IRuntimeNode
	{
		/// <summary>
		/// Does this runtime node pause the graph processing and forces
		/// the <see cref="DialogueRuntimeGraph"/> to wait till processing the next node.
		/// </summary>
		public bool ShouldPauseGraphProcessing { get; set; }

		/// <summary>
		/// The conversation this node is a part of.
		/// </summary>
		public DialogueConversation Conversation { get; set; }
		
		/// <summary>
		/// Goes through <see cref="RuntimeNode"/> and precalculate the current branch the dialogue should follow.
		/// </summary>
		/// <returns></returns>
		public void TraverseNode(in List<RuntimeNode> branchNodes);
		
		/// <summary>
		/// Processes the logic for custom nodes.
		/// </summary>
		public void ProcessNode();
	}
	
	/// <summary>
	/// The data representation of a Dialogue Node for runtime execution.
	/// </summary>
	[System.Serializable]
    public class RuntimeNode : IRuntimeNode
    {
	    [field:SerializeField] public bool ShouldPauseGraphProcessing { get; set; }
	    [field:SerializeField] [field:HideInInspector] public DialogueConversation Conversation { get; set; }
	    
	    public bool IsBlockNode { get; set; }


	    public virtual void TraverseNode(in List<RuntimeNode> branchNodes)
	    {
		    Debug.Log($"Node of Type: {GetType()} has not implemented traversal logic yet.");
	    }

	    /// <summary>
	    /// Processes the logic for custom nodes.
	    /// </summary>
	    public virtual void ProcessNode()
	    {
		    Debug.Log($"Node of Type: {GetType()} has not implemented process logic yet.");
	    }

	    public RuntimeNode ShallowCopy()
	    {
		    return (RuntimeNode)MemberwiseClone();
	    }
    }
}
