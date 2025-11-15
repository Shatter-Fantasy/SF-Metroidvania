using SF.Characters.Controllers;
using UnityEngine;

namespace SF.Characters.Physics
{
    /// <summary>
    /// The current physics properties for a character. This is used in all <see cref="RigidbodyController2D"/> and classes that inherit from it.
    /// Also used in <see cref="SF.Physics.PhysicsVolume"/> which allows creating new states of physics like low gravity, swimming, gliding in the air.
    /// </summary>
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
		public float GroundDeceleration = 1.2f;
    }
}
