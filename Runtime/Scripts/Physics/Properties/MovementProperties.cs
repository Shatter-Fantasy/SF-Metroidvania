using UnityEngine;

namespace SF.U2D.Physics
{
    /// <summary>
    /// An interface to define a set of properties for different types of <see cref="PhysicController2D"/>
    /// </summary>
    public interface IPhysicsControllerProperties 
    { 
        // no-op
    }

    [System.Serializable]
    public struct FlyingPhysicsProperties : IPhysicsControllerProperties
    {
        
    }

    // TODO: Rename this struct to GroundedPhysicsProperties
    [System.Serializable]
    public struct MovementProperties() : IPhysicsControllerProperties
    {
        public float GroundSpeed = 8f;
        public float GroundRunningSpeed = 7.5f;
        public float GroundAcceleration = 1f;
        public float GroundDeacceleration = 1f;
        public float GroundMaxSpeed = 10f;

        public float GravityScale = 1f;
        public float TerminalVelocity = 20f;
        public float MaxUpForce = 15f;

        public Vector2 ClimbSpeed = new Vector2(1,1);
    }
}