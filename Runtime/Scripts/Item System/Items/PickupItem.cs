using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

namespace SF.ItemModule
{
    using Characters.Controllers;
    using Interactables;
    using Managers;
    using PhysicsLowLevel;
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
        
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent, SFShapeComponent callingShapeComponent)
        {
            if (GameManager.Instance.ControlState == GameControlState.Cutscenes)
                return;

            if (beginEvent.GetCallbackComponentOnVisitor<SFShapeComponent>()
                       .TryGetComponent(out PlayerController controller))
            {
                Interact(controller);
            }
        }

        public void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent, SFShapeComponent callingShapeComponent)
        { 
            // noo - No Operation
        }
    }
}
