using UnityEngine;
using UnityEngine.InputSystem;
using SF.InputModule;
using SF.Managers;

namespace SF.AbilityModule.Characters
{
    public class HorizontalMovementAbility : AbilityCore, IInputAbility
    {
        [SerializeField] private bool _isRunningToggleable = true;

        protected override void OnInitialize()
        {
			if(_isRunningToggleable)
			{
				_controller2d.ReferenceSpeed = _controller2d.IsRunning
					? _controller2d.CurrentPhysics.GroundRunningSpeed
					: _controller2d.CurrentPhysics.GroundSpeed;
			}
        }
        #region Input Actions
        private void OnInputMove(InputAction.CallbackContext context)
        {
	        if (!CanStartAbility())
		        return;
	        
			Vector2 input = context.ReadValue<Vector2>();
			float xDirection = input.x != 0 ? input.x : 0;
			_controller2d.Direction = new Vector2(xDirection, _controller2d.Direction.y);
		}
        private void OnMoveInputRun(InputAction.CallbackContext context)
        {
            _controller2d.IsRunning = (_isRunningToggleable)
				? !_controller2d.IsRunning
				: context.ReadValue<float>() > 0;
                
			_controller2d.ReferenceSpeed = _controller2d.IsRunning
                    ? _controller2d.CurrentPhysics.GroundRunningSpeed
                    : _controller2d.CurrentPhysics.GroundSpeed;
        }

		private void OnMoveInputRunCancelled(InputAction.CallbackContext context)
        {
			if(_isRunningToggleable)
				return;

			_controller2d.IsRunning = false;
			_controller2d.ReferenceSpeed = _controller2d.CurrentPhysics.GroundSpeed;
		}
        #endregion Input Actions
        private void OnEnable()
		{
			InputManager.Controls.Player.Enable();
			InputManager.Controls.Player.Move.performed += OnInputMove;
			InputManager.Controls.Player.Move.canceled += OnInputMove;

            InputManager.Controls.Player.Running.performed += OnMoveInputRun;
			InputManager.Controls.Player.Running.canceled += OnMoveInputRunCancelled;
		}

        private void OnDisable()
		{
			if(InputManager.Controls == null) return;

			InputManager.Controls.Player.Move.performed -= OnInputMove;
			InputManager.Controls.Player.Move.canceled -= OnInputMove;

            InputManager.Controls.Player.Running.performed -= OnMoveInputRun;
			InputManager.Controls.Player.Running.canceled -= OnMoveInputRunCancelled;
		}
	}
}