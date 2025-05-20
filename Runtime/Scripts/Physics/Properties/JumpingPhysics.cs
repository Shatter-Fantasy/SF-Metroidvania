namespace SF
{
    [System.Serializable]
    public struct JumpingPhysics
    {
        public float JumpHeight;
        public int JumpAmount;
        public int JumpsRemaining;

        public JumpingPhysics(float _jumpheight = 7f,
            int _jumpAmount = 1)
        {
            JumpHeight = _jumpheight;
            JumpAmount = _jumpAmount;
            JumpsRemaining = _jumpAmount;
        }

        public void ResetJumps()
        {
            JumpsRemaining = JumpAmount;
        }
    }
}
