using SF.Characters.Controllers;
using SF.Interactables;
using SF.Managers;
using SF.PhysicsLowLevel;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.ItemModule
{
    public class PickupItem : MonoBehaviour, 
        IInteractable<PlayerController>, 
        ITriggerShapeCallback
    {
        [field: SerializeField] public InteractableMode InteractableMode { get; set; }
        
        public ItemData Item;

        private void Start()
        {
            if (TryGetComponent(out SFShapeComponent component))
                component.AddTriggerCallbackTarget(this);
        }

        public void Interact()
        {
            
        }

        public void Interact(PlayerController controller)
        {
            if(controller == null || Item == null)
                return;
           
            // Make sure we added an instantiated inventory to the player first.
            if(controller.TryGetComponent(out PlayerInventory playerInventory))
            {
                PickUpItem(playerInventory);
            }
        }

        private void PickUpItem(PlayerInventory playerInventory)
        {         
            playerInventory.AddItem(Item.ID);
            Destroy(gameObject);
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;
           
            if (beginEvent.visitorShape.callbackTarget is not PlayerController body2D)
                return;
                    
            if(body2D.CollisionInfo.CollisionActivated)
            {
                Interact(body2D);
            }
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent)
        {
            // noo - No Operation
        }

        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            OnTriggerBegin2D(beginEvent);
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        { 
            // noo - No Operation
        }
    }
}
