using UnityEngine;
using UnityEngine.InputSystem;

using SF.Characters.Controllers;
using SF.InputModule;
using UnityEngine.LowLevelPhysics2D;

namespace SF.Interactables
{
    public class PlayerInteractionController : InteractionController
    {
        public void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent)
        {
            Debug.Log("Player Interaction Controller");
            
            if (!((GameObject)beginEvent.visitorShape.GetOwner()).TryGetComponent(out IInteractable interactable)
                || interactable.InteractableMode != InteractableMode.Collision)
            {
                return;
            }
            
            
            if(gameObject.TryGetComponent(out PlayerController controller))
            {
                if (interactable is IInteractable<PlayerController> interactableController)
                {
                    interactableController.Interact(controller);
                }
            }
            else
            {
                interactable.Interact();
            }
        }
        
        protected void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if(_boxCollider2D == null) return;

            _hitColliders = new Collider2D[_hitColliders.Length];
            
            Physics2D.OverlapBox((Vector2)transform.position, _boxCollider2D.bounds.size, 0f, _interactableFilter, _hitColliders);

            for (int i = 0; i < _hitColliders.Length; i++)
            {
                // Make sure if we do find something we can interact with, it is set to input mode. 
                if (_hitColliders[i] == null
                    || !_hitColliders[i].TryGetComponent(out IInteractable interactable)
                        || interactable.InteractableMode != InteractableMode.Input)
                    return;
                
                // If we are interacting with something needing to know any data about our player send the PlayerController. 
                if(gameObject.TryGetComponent(out PlayerController controller) && 
                   interactable is IInteractable<PlayerController> interactableController)
                {
                    interactableController.Interact(controller);
                }
                else
                {
                    interactable.Interact();
                }
            }
        }
        
        private void OnEnable()
        {
            InputManager.Controls.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            InputManager.Controls.Player.Interact.performed -= OnInteractPerformed;
        }
        
    }
}
