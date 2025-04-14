using SF.Characters.Controllers;
using SF.Interactables;
using UnityEngine;

namespace SF.DialogueModule
{
    public class DialogueImprov : MonoBehaviour, IInteractable
    {
        public int ConversationGUID;
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }
       
        public void Interact() {  }
        public void Interact(PlayerController controller)
        {
            DialogueManager.TriggerConversation(ConversationGUID);
        }
    }
}