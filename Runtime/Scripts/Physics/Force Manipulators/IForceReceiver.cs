using UnityEngine;

namespace SF
{
    public interface IForceReciever
    {
        public abstract void SetExternalVelocity(Vector2 velocity);
    }
}
