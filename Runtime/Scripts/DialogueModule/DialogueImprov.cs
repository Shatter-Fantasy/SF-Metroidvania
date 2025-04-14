using System;
using SF.Characters.Controllers;
using SF.Interactables;
using UnityEngine;

namespace SF.DialogueModule
{
    public class DialogueImprov : MonoBehaviour, IInteractable
    {
        public int ConversationGUID;
        [SerializeField] private DialogueConversation _dialogueConversation;
        [SerializeField] private DialogueDatabase _dialogueDB;
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }


        private void Start()
        {
            if (_dialogueDB == null)
                return;

            _dialogueConversation = _dialogueDB.Conversations.Find(x => x.GUID == ConversationGUID);
        }

        public void Interact() {  }

        public void Interact(PlayerController controller)
        {
            if (_dialogueConversation != null)
            {
                DialogueEvent.Trigger(DialogueEventTypes.DialoguePopup, _dialogueConversation.NextDialogueEntry());
            }
            else
                DialogueEvent.Trigger(DialogueEventTypes.DialoguePopup,"No conversation for the Guid was found.");
        }
    }
}