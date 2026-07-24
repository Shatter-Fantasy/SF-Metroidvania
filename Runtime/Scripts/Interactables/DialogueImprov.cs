using System;
using UnityEngine;
using Unity.U2D.Physics;

namespace SF.DialogueModule
{
    using Characters.Controllers;
    using Interactables;
    using U2D.Physics;
    
    public class DialogueImprov : MonoBehaviour, 
        IInteractable<PlayerController>,
        ITriggerShapeBegin2DCallback, ITriggerShapeEnd2DCallback
    {
        [Header("Set either the guid or the conversation asset.")]
        public int ConversationGUID;
        [field:SerializeField] private DialogueConversation _dialogueConversation;
        
        [field:Space()]
        [field:SerializeField] public InteractableMode InteractableMode { get; set; }

        [field:SerializeField] private SFShapeComponent _shapeComponent;
        
        private void Awake()
        {
            _shapeComponent?.AddTriggerCallbackTarget(this);
        }

        public void Interact() {  }
        public void Interact(PlayerController controller)
        {
#if SF_DIALOGUE_GRAPH
            if(_dialogueConversation != null)
                DialogueManager.TriggerConversation(_dialogueConversation,this);
            else
                DialogueManager.TriggerConversation(ConversationGUID,this);
#endif
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            if(_dialogueConversation != null)
                DialogueManager.TriggerConversation(_dialogueConversation,this);
            else
                DialogueManager.TriggerConversation(ConversationGUID,this);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        {
            DialogueManager.StopConversation();
        }
    }
}