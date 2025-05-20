using UnityEngine;

namespace SF.Characters.Physics
{
    [System.Serializable]
    public class CharacterPhysics
    {
        [Header("Grounded Speed")]
        public float WalkSpeed = 5f;
        public float RunSpeed = 9f;

        [Header("Ground Acceleration")]
        [Tooltip("How much speed per second you gain while going from walk to run")]
        public float GroundAcceleration = 1.2f;
		[Tooltip("How much speed per second you lose while going from run to walk")]
		public float GroundDeacceleration = 1.2f;
    }
}
