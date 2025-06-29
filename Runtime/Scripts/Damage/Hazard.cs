using UnityEngine;

namespace SF
{
    [System.Flags]
    public enum Direction : int
    {
        Any = 0,
        Left = 1,
        Right = 2,
        Sides = 3,
        Up = 4,
        Down = 8,
    }

    public class Hazard : MonoBehaviour, IDamage
    {
        private Collider2D _collider2D;
        private Vector2 _collisionNormal;
        public Direction DamageDirection;
        public int DamageAmount = 1;
        [SerializeField] private Vector2 _knockBackForce;

        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if(collision2D.gameObject.TryGetComponent(out IDamagable damagable))
            {
                _collisionNormal = collision2D.GetContact(0).normal;
                if(CheckCollisionDirection())
                    damagable.TakeDamage(DamageAmount,_knockBackForce);
            }
        }

        private void OnTriggerEnter2D(Collider2D col2D)
        {
            if(col2D.TryGetComponent(out IDamagable damagable))
            {
                _collisionNormal = col2D.Distance(_collider2D).normal;
                
                if(CheckCollisionDirection())
                    damagable.TakeDamage(DamageAmount,_knockBackForce);
            }
        }

        private bool CheckCollisionDirection()
        {
            switch(DamageDirection)
            {
                case Direction.Any:
                    return true;
                case Direction.Left:
                    if(_collisionNormal.x < 0) 
                        return true;
                    break;
                case Direction.Right:
                    if(_collisionNormal.x > 0)
                        return true;
                    break;
                case Direction.Sides:
                    if(_collisionNormal.x < 0 || _collisionNormal.x > 0)
                        return true;
                    break;
                case Direction.Up:
                    if(_collisionNormal.y > 0)
                        return true;
                    break;
                case Direction.Down:
                    if(_collisionNormal.y < 0)
                        return true;
                    break;
            }

            return false;
        }
    }
}
