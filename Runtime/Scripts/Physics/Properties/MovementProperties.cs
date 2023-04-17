namespace SF.Physics
{
    [System.Serializable]
    public struct MovementProperties
    {
        [UnityEngine.Header("Gound Physics")]
        public float GroundSpeed;
        public float GroundAcceleration;
        public float GroundMaxSpeed;

        [UnityEngine.Header("Air Physics")]
        public float Gravity;
        public float GravityAcceleration;
        public float TerminalVelocity;

        [UnityEngine.Header("Jump Physics")]
        public float JumpHeight;
        public MovementProperties(
            float _groundSpeed = 5f,
            float _groundAcceleration = 1f,
            float _groundMaxSpeed = 10f,
            float _gravity = -9.81f,
            float _gravityAcceleration = 1f,
            float _terminalVelocity = 20f,
            float _jumpheight = 8f)
        {
            GroundSpeed = _groundSpeed;
            GroundAcceleration = _groundAcceleration;
            GroundMaxSpeed = _groundMaxSpeed;

            Gravity = _gravity;
            GravityAcceleration = _gravityAcceleration;
            TerminalVelocity = _terminalVelocity;

            JumpHeight = _jumpheight;
		}
    }
}