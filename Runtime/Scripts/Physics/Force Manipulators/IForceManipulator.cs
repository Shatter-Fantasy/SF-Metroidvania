using UnityEngine;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Implement this to customize force and velocity interactions with an object that implements <see cref="IForceReciever"/>.
    /// </summary>
    public interface IForceManipulator
    {
        public void ExtertForce(IForceReciever forceReceiver, Vector2 force);
    }
}
