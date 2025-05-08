using SF.Characters.Controllers;
using SF.InputModule;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SF.Interactables
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private ContactFilter2D _interactableFilter;
        private BoxCollider2D _boxCollider2D;

        [SerializeField] private Collider2D[] _hitColliders = new Collider2D[5];
        
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
            
                
            Physics2D.OverlapBox((Vector2)transform.position, _boxCollider2D.bounds.size, 0f, _interactableFilter, _hitColliders);

            for (int i = 0; i < _hitColliders.Length; i++)
            {
                if(_hitColliders[i] != null && _hitColliders[i] .TryGetComponent(out IInteractable interactable))
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
