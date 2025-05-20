using SF.ProjectileModule;
using UnityEngine;

namespace SF.PowerUpModule
{
    [System.Serializable]
    public class FireBallPowerUp : IPowerUp
    {
        public Projectile PowerUpProjectile;
        [HideInInspector] public Transform ProjectileSpawn;
        public FireBallPowerUp(Transform projectileSpawn)
        {
            ProjectileSpawn = projectileSpawn;
        }
        public void Activate()
        {
            if(PowerUpProjectile != null && ProjectileSpawn != null)
                PowerUpProjectile.Fire(ProjectileSpawn.position);
        }
    }
}
