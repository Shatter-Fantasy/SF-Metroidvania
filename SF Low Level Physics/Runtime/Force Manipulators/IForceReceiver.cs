using UnityEngine;

namespace SF.PhysicsLowLevel
{
    /// <summary>
    ///  Allows objects to have a custom implementation for force and velocity interactions for physics.
    /// </summary>
    /// <remarks>
    ///  Example usage of this is seen in the interface <see cref="IForceManipulator"/>. 
    /// </remarks>
    public interface IForceReceiver
    {
        public abstract void SetExternalVelocity(Vector2 velocity);
    }
}
