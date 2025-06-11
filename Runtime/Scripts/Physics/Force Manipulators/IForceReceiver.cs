using UnityEngine;

namespace SF.Physics
{
    public interface IForceReciever
    {
        public abstract void SetExternalVelocity(Vector2 velocity);
    }
}
