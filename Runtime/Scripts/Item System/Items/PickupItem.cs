using SF.Characters.Controllers;
using SF.Interactables;
using SF.InventoryModule;

using UnityEngine;

namespace SF.ItemModule
{
    public class PickupItem : MonoBehaviour, IInteractable<PlayerController>
    {
        [field: SerializeField] public InteractableMode InteractableMode { get; set; }
        
        public ItemData Item;
        
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
                playerInventory.PickUpItem(Item.ID);
                Destroy(gameObject);
            }
        }
    }
}
