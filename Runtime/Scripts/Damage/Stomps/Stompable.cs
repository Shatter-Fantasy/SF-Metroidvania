using UnityEngine;

namespace SF.DamageModule
{
    public class Stompable : MonoBehaviour, IStompable
    {
        private IDamagable _health;
        private void Awake()
        {
            TryGetComponent(out _health);
        }
        public void Stomp()
        {
            if (_health == null)
                return;

            _health.TakeDamage(1);
        }
    }
}
