using System.Collections.Generic;
using UnityEngine;

namespace SF.DialogueModule.Nodes
{
	[System.Serializable]
    public class ConversationEntryRuntimeNode : RuntimeNode
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

	    public override void TraverseNode(in List<RuntimeNode> branchNodes)
	    {
		    _dialogueEntry = new DialogueEntry(Text,SpeakerName);
		    branchNodes.Add(this);
	    }

	    public override void ProcessNode()
	    { 
		    DialogueManager.UpdateDialogueText(_dialogueEntry);
	    }
    }
}