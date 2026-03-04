using UnityEngine;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    /// Implement this to customize force and velocity interactions with an object that implements <see cref="IForceReceiver"/>.
    /// </summary>
    public interface IForceManipulator
    {
        public void ExtertForce(IForceReceiver forceReceiver, Vector2 force);
    }
}
