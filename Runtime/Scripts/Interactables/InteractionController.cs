using UnityEngine;

namespace SF.Interactables
{
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] protected ContactFilter2D _interactableFilter;
        protected BoxCollider2D _boxCollider2D;

        [SerializeField] protected Collider2D[] _hitColliders = new Collider2D[5];
        
        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IInteractable interactable) 
                && interactable.InteractableMode == InteractableMode.Collision)
            {
                interactable.Interact();
            }
        }
    }
}
