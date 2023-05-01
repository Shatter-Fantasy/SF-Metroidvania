using UnityEngine;
using UnityEngine.InputSystem;

namespace SF.Characters.Controllers
{
    public class PlayerController : GroundedController2D
    {
		#region Input Actions
		private void OnInputMove(InputAction.CallbackContext context)
		{
			Vector2 input = context.ReadValue<Vector2>();
			AddHorizontalForce(input.x);
		}
		
		private void OnInputJump(InputAction.CallbackContext context)
		{
			AddVerticalForce(CurrentPhysics.JumpHeight);
		}
		#endregion
		private void OnEnable()
		{
			InputManager.Controls.Player.Enable();
			InputManager.Controls.Player.Move.performed += OnInputMove;
			InputManager.Controls.Player.Jump.performed += OnInputJump;
		}
		private void OnDisable()
		{
			InputManager.Controls.Player.Move.performed -= OnInputMove;
			InputManager.Controls.Player.Jump.performed -= OnInputJump;
		}
	}
}