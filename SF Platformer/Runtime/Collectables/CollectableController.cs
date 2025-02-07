using UnityEngine;

namespace SF.CollectableModule
{
    public class CollectableController : MonoBehaviour
    {
        public void OnTriggerEnter2D(Collider2D collider2D)
        {
            
            if(collider2D.TryGetComponent(out ICollectable collectable))
            {
                collectable.Collect();
            }
        }
    }
}
