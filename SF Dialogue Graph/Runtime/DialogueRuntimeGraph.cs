using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SF.DialogueModule;
using UnityEngine.Serialization;

namespace SF.DialogueModule.Nodes
{
	
	/// <summary>
	/// The runtime representation of a dialogue graph.
	/// </summary>
	/// <remarks>
	/// This is built from a list of dialogue nodes from the editor graph asset See SFEditor.Dialogue.Graphs.DialogueGraphImporter.
	/// Go to the editor assembly folder for the dialogue system to find the class.
	/// </remarks>
	[Serializable]
    public class DialogueRuntimeGraph
    {
	    /// <summary>
	    /// The runtime nodes for the <see cref="DialogueRuntimeGraph"/>
	    /// </summary>
	    /// <remarks>
	    ///	The constructor is called during an ScriptedImporter. See SFEditor.Dialogue.Graphs.DialogueGraphImporter.
	    /// so we do not say public List{IRuntimeNode} Nodes = new();
	    /// If this is done than the scripted imported will erase the
	    /// SerializeReference nodes when opening Unity losing all data.
	    /// </remarks>
	    [SerializeReference]
	    public List<IRuntimeNode> Nodes = new();

	    /// <summary>
	    /// The nodes for the current branch of a conversation.
	    /// </summary>
	    [SerializeReference]
	    public List<RuntimeNode> BranchNodes = new ();
	    
	    /// <summary>
	    /// Is the graph processing paused and waiting for a node to complete something.
	    /// </summary>
	    public bool IsPaused;
	    
	    [NonSerialized] public DialogueConversation ClonedConversation;
	    [NonSerialized] public DialogueConversation OriginalConversation;
	    
	    [NonSerialized] public List<DialogueExtensionBase> Extensions = new();
	    [NonSerialized] public List<DialogueExtensionBase> StartStageExtensions = new();
	    [NonSerialized] public List<DialogueExtensionBase> CompletedStageExtensions = new();
	    [NonSerialized] public List<DialogueExtensionBase> ConversaNodeStageExtensions = new();
	    
	    /// <summary>
	    /// Called everytime the DialogueGraphImporter runs.
	    /// This means on opening Unity as well and script comping/reloading domain.
	    /// </summary>
	    public DialogueRuntimeGraph()
	    {
		   
	    }
	    
	    public DialogueRuntimeGraph(DialogueConversation conversation)
	    {
		    ClonedConversation = conversation.Clone(conversation);
		    OriginalConversation = conversation;
		    ClonedConversation.RuntimeGraph = this;
		    Nodes = ClonedConversation.Nodes;
	    }

	    public DialogueRuntimeGraph(DialogueConversation conversation, List<IRuntimeNode> nodes)
	    {
		    ClonedConversation = conversation.Clone(conversation);
		    OriginalConversation = conversation;
		    ClonedConversation.RuntimeGraph = this;
		    
		    Nodes = nodes;
	    }
	    
	    public void GetCurrentBranch()
	    {
		    BranchNodes.Clear();
		    foreach (var runtimeNode in Nodes)
		    {
			    if (runtimeNode is RuntimeNode dialogueRuntimeNode)
			    {
				    dialogueRuntimeNode.TraverseNode(BranchNodes);
			    }
		    }
	    }

	    public void ProcessExtensions(in Component callingComponent)
	    {
		    Extensions.Clear();
		    StartStageExtensions.Clear();
		    CompletedStageExtensions.Clear();
		    ConversaNodeStageExtensions.Clear();
		    
		    if (callingComponent != null)
		    {
			    Extensions = callingComponent.GetComponents<DialogueExtensionBase>().ToList();

			    foreach (var extension in Extensions)
			    {
				    extension.RuntimeGraph = this;
				    switch (extension.Stage)
				    {
					    case ConversationStage.Start:
						    StartStageExtensions.Add(extension);
						    break;
					    case ConversationStage.Completed:
						    CompletedStageExtensions.Add(extension);
						    break;
					    case ConversationStage.DialogueNode:
						    ConversaNodeStageExtensions.Add(extension);
						    break;
					    default:
						    throw new ArgumentOutOfRangeException();
				    }
			    }
		    }
		    
		    foreach (var extension in StartStageExtensions)
		    {
			    extension.ControlFlow();
		    }
	    }

	    /// <summary>
	    /// Processes the nodes in the current <see cref="BranchNodes"/>.
	    /// </summary>
	    public async Awaitable ProcessNodeBranch()
	    {
		    for (int i = 0; i < BranchNodes.Count; i++)
		    {
			    BranchNodes[i].ProcessNode();

			    IsPaused = BranchNodes[i].ShouldPauseGraphProcessing;
			    
			    while (IsPaused)
			    {
				    await Awaitable.NextFrameAsync();
			    }
		    }
		    
		    // All nodes in the dialogue are processed, so the conversation has ended.
		    DialogueManager.StopConversation();
	    }
    }
}
