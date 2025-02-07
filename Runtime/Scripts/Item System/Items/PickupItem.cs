using SF.Characters.Controllers;
using SF.Interactables;
using SF.InventoryModule;

using UnityEngine;

namespace SF.ItemModule
{
    public class PickupItem : MonoBehaviour, IInteractable
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
           
            // Make sure we added a instantiated inventory to the player first.
            if(controller.TryGetComponent(out PlayerInventory playerInventory))
            {
                PickUpItem(playerInventory);
            }
        }

        private void PickUpItem(PlayerInventory playerInventory)
        {          
            playerInventory.Items.Add(Item);
            Destroy(gameObject);
        }
    }
}
