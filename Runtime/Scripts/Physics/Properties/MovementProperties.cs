
using UnityEngine;

namespace SF.PhysicsLowLevel
{
    [System.Serializable]
    public struct MovementProperties
    {
        public float GroundSpeed;
        public float GroundRunningSpeed;
        public float GroundAcceleration;
        public float GroundDeacceleration;
        public float GroundMaxSpeed;

        public float GravityScale;
        public float TerminalVelocity;
        public float MaxUpForce;

        public Vector2 ClimbSpeed;

        public MovementProperties(
            Vector2 _climbSpeed,
            float _groundSpeed = 5f,
            float _groundRunningSpeed = 7.5f,
            float _groundAcceleration = 1f,
            float _groundDeacceleration = 1f,
            float _groundMaxSpeed = 10f,
            float _gravityScale = 1,
            float _terminalVelocity = 20f,
            float _maxUpForce = 15f)
        {
            GroundSpeed = _groundSpeed;
            GroundRunningSpeed = _groundRunningSpeed;
            GroundAcceleration = _groundAcceleration;
            GroundDeacceleration = _groundDeacceleration;
            GroundMaxSpeed = _groundMaxSpeed;

            GravityScale = _gravityScale;
            TerminalVelocity = _terminalVelocity;
            MaxUpForce = _maxUpForce;

            ClimbSpeed = _climbSpeed;
        }
    }
}