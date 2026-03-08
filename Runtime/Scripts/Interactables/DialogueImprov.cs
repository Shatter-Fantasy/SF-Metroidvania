using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.DialogueModule
{
    using Characters.Controllers;
    using Interactables;
    using SF.PhysicsLowLevel;
    
    public class DialogueImprov : MonoBehaviour, 
        IInteractable<PlayerController>,
        ITriggerShapeCallback
    {
        public int ConversationGUID;
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }
        
        public void Interact() {  }
        public void Interact(PlayerController controller)
        {
            DialogueManager.TriggerConversation(ConversationGUID);
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            DialogueManager.TriggerConversation(ConversationGUID);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            DialogueManager.StopConversation();
        }
    }
}