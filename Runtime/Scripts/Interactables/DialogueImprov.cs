using UnityEngine;
using Unity.U2D.Physics;

namespace SF.DialogueModule
{
    using Characters.Controllers;
    using Interactables;
    using U2D.Physics;
    
    public class DialogueImprov : MonoBehaviour, 
        IInteractable<PlayerController>,
        ITriggerShapeCallback
    {
        public int ConversationGUID;
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }

        

        public void Interact() {  }
        public void Interact(PlayerController controller)
        {
#if SF_DIALOGUE_GRAPH
            DialogueManager.TriggerConversation(ConversationGUID);
#endif
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
#if SF_DIALOGUE_GRAPH
            DialogueManager.TriggerConversation(ConversationGUID);
#endif
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
#if SF_DIALOGUE_GRAPH
            DialogueManager.StopConversation();
#endif
        }
    }
}