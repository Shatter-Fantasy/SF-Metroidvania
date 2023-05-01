using UnityEngine;

using SF.Physics;

namespace SF.Characters.Controllers
{
    public class GroundedController2D : Controller2D
    {
		[Header("States")]
		public CharacterState CharacterState;
		public MovementState MovementState;

		[Header("Physics Properties")]
		public MovementProperties DefaultPhysics;
		public MovementProperties CurrentPhysics;

		[Header("Contact Filters")]
		public ContactFilter2D GroundFilter;

		[Header("Booleans")]
		public bool IsGrounded = false;
		public bool IsFalling = false;
		public bool IsSwimming = false;


		#region Collision Calculations
		protected virtual void GroundChecks()
		{

		}
		#endregion

		protected override void CalculateHorizontal()
		{
			_calculatedVelocity.x = _controllerForce.x * (Velocity.x + CurrentPhysics.GroundSpeed);
		}
		protected override void CalculateVertical()
		{
			_calculatedVelocity.y = _controllerForce.y + (Velocity.y);

			if(!IsGrounded)
			{
				_calculatedVelocity.y += CurrentPhysics.Gravity;

				if(_calculatedVelocity.y < 0)
				{
					_calculatedVelocity.y = (_calculatedVelocity.y < (-1 * CurrentPhysics.TerminalVelocity)
						? CurrentPhysics.TerminalVelocity
						: _calculatedVelocity.y);
				}
				else if(_calculatedVelocity.y > 0)
				{
					_calculatedVelocity.y = (_calculatedVelocity.y < (CurrentPhysics.MaxUpForce)
						? CurrentPhysics.MaxUpForce
						: _calculatedVelocity.y);
				}
			}
		}
		public virtual void UpdatePhysics(MovementProperties movementProperties)
		{
			CurrentPhysics = movementProperties;
		}
	}
}
