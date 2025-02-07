using UnityEngine;

namespace SF.ProjectileModule
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        public float ProjectileSpeed = 5f;

        private bool _wasInitialized = false;

        private Rigidbody2D _RGB;

        public void Init()
        {
            _RGB = GetComponent<Rigidbody2D>();
            _wasInitialized = true;
        }

        public void Fire(Vector2 spawnPosition)
        {
            if(!_wasInitialized)
                Init();

            _RGB.linearVelocity = new Vector2(ProjectileSpeed, 0);
        }

        private void OnBecameInvisible()
        {
            // TODO:Make this into a pool system using a custom ObjectPool class to inject components onto the pooled game objects.
            // gameObject.SetActive(false);

            Destroy(gameObject);
        }
    }
}
