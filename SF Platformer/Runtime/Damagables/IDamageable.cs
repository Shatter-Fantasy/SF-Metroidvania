using UnityEngine;

namespace SF
{
    public interface IDamagable 
    {
        void TakeDamage(int damage, Vector2 knockback = new Vector2());
        void InstantKill();
    }
}
