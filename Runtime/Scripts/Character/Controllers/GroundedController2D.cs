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
	}
}
