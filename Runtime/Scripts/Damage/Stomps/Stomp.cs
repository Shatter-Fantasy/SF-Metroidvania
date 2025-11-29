using SF.Characters.Controllers;
using SF.PhysicsLowLevel;
using UnityEngine;

namespace SF
{
    public class Stomp : MonoBehaviour
    {
        public float BounceVelocity = 5f;
        private Collider2D _collider2d;
        private ControllerBody2D _controller2D;

        private void Awake()
        {
            _collider2d = GetComponent<Collider2D>();
            _controller2D = GetComponent<ControllerBody2D>();
        }
        public void OnTriggerEnter2D(Collider2D other)
        {
            if(other.TryGetComponent(out IStompable stompable))
            {
                if(_collider2d.Distance(other).normal.y < 0)
                    stompable.Stomp();

                if (_controller2D == null)
                    return;

                _controller2D.SetVerticalVelocity(BounceVelocity);
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.collider.TryGetComponent(out IStompable stompable))
            {
                if(_collider2d.Distance(collision.collider).normal.y < 0)
                {
                    stompable.Stomp();
                }
            }
        }
    }
}
