using UnityEngine;
using SF.DialogueModule;

namespace SF.CommandModule
{

    public enum DialogueCommandType
    {
        StartDialogue,
        EndDialogue,
        SkipDialogue
    }
    
    [System.Serializable]
    [CommandMenu("Dialogue/Conversation Control")]
    public class DialogueCommand : CommandNode, ICommand
    {
        [SerializeField] private int _conversationGUID;
        private DialogueConversation _conversation;
        
        protected override bool CanDoCommand()
        {
            IsAsyncCommand = false;
            return DialogueManager.Instance.DialogueDB.GetConversation(_conversationGUID, out _conversation);
        }

        protected override void DoCommand()
        {
            DialogueManager.TriggerConversation(_conversation.GUID);
        }

        protected override Awaitable DoAsyncCommand() { return null; }
    }
}
