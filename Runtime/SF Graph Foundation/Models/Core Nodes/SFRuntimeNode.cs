using System.Collections.Generic;
using UnityEngine;

namespace SF.Graphs.Nodes
{


	/// <summary>
	///  Base interface for all editor and runtime sf graph related nodes.
	/// This needs moved into the runtime assembly sooner or later.
	/// </summary>
	public interface ISFNode
	{
		// Purposely left blank.
	}

	/// <summary>
	/// The data representation of a Dialogue Node for runtime execution.
	/// </summary>
	[System.Serializable]
    public class SFRuntimeNode : ISFNode
    {
	    public bool ShouldPauseGraphProcessing;

	    public bool IsBlockNode;


	    public virtual void TraverseNode(in List<SFRuntimeNode> branchNodes)
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

	    public SFRuntimeNode ShallowCopy()
	    {
		    return (SFRuntimeNode)MemberwiseClone();
	    }
    }
}