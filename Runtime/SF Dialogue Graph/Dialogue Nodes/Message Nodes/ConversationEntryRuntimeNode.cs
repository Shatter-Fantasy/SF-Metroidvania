using System.Collections.Generic;

namespace SF.DialogueModule.Nodes
{
	using SF.Graphs.Nodes;

	[System.Serializable]
    public class ConversationEntryRuntimeNode : SFRuntimeNode
    {
	    public string Text;
	    public string SpeakerName;
	    
	    private DialogueEntry _dialogueEntry;

	    public ConversationEntryRuntimeNode()
	    {
		    IsBlockNode = true;
		    ShouldPauseGraphProcessing = true;
	    }

	    public ConversationEntryRuntimeNode(string text, string speakerName = "")
	    {
		    IsBlockNode = true;
		    Text = text;
		    SpeakerName = speakerName;
		    
		    ShouldPauseGraphProcessing = true;
	    }
	    
	    public static implicit operator DialogueEntry(in ConversationEntryRuntimeNode conversationNode)
	    {
		    return new DialogueEntry(conversationNode.Text,conversationNode.SpeakerName);
	    }

	    public static explicit operator ConversationEntryRuntimeNode(in DialogueEntry dialogueEntry)
	    {
		   return new ConversationEntryRuntimeNode(dialogueEntry.Text,dialogueEntry.SpeakerName);
	    }

		/// <summary>
		/// Checks to see what all nodes needs processed for the current branch of dialogue.
		/// </summary>
		/// <param name="branchNodes"></param>
	    public override void TraverseNode(in List<SFRuntimeNode> branchNodes)
	    {
		    _dialogueEntry = new DialogueEntry(Text,SpeakerName);
	    }

	    public override void ProcessNode()
	    { 
		    DialogueManager.UpdateDialogueText(_dialogueEntry);
	    }
    }
}