using UnityEngine;

namespace SF.Weapons.ProjectileModule
{
    public interface IProjectile
    {
        void Init();
        void Fire(Vector2 spawnPoint);
    }
    
}
