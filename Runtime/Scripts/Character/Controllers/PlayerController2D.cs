using UnityEngine;

using SF.Physics;

namespace SF.Characters.Controllers
{
    public class PlayerController : GroundedController2D
    {

		public bool IsGrounded = false;
		public bool IsFalling = false;
		public bool IsSwimming = false;

		#region Collision Calculations
		protected virtual void GroundChecks()
		{

		}
		#endregion


		public void UpdatePhysics(MovementProperties movementProperties)
		{
			CurrentPhysics = movementProperties;
		}
	}
}
