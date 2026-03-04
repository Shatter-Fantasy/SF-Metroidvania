using UnityEngine;

namespace SF.DialogueModule
{
    using Characters.Controllers;
    using Interactables;
    public class DialogueImprov : MonoBehaviour, IInteractable<PlayerController>
    {
        public int ConversationGUID;
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }
        
        public void Interact() {  }
        public void Interact(PlayerController controller)
        {
            DialogueManager.TriggerConversation(ConversationGUID);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                DialogueManager.StopConversation();
            }
        }
    }
}