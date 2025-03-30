using SF.Characters.Controllers;
using SF.InputModule;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SF.Interactables
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private LayerMask _interactableLayers;
        private BoxCollider2D _boxCollider2D;

        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IInteractable interactable))
            {
                if(interactable.InteractableMode == InteractableMode.Collision)
                {
                    if(gameObject.TryGetComponent(out PlayerController controller))
                    {
                        interactable.Interact(controller);
                    }
                    else
                    {
                        interactable.Interact();
                    }
                }
            }

        }

        private void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if(_boxCollider2D == null) return;

            var col2D = Physics2D.OverlapBox(transform.position, _boxCollider2D.bounds.size, 0, _interactableLayers);


            if(col2D != null && col2D.TryGetComponent(out IInteractable interactable))
            {
                if(interactable.InteractableMode == InteractableMode.Input)
                {
                    if(gameObject.TryGetComponent(out PlayerController controller))
                    {
                        interactable.Interact(controller);
                    }
                    else
                    {
                        interactable.Interact();
                    }
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
