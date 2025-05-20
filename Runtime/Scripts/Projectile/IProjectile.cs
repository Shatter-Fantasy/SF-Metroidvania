using UnityEngine;

namespace SF.ProjectileModule
{
    public interface IProjectile
    {
        void Init();
        void Fire(Vector2 spawnPoint);
    }
    
}
