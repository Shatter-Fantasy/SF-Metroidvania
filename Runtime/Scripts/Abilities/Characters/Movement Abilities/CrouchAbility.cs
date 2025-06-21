using UnityEngine;
using UnityEngine.InputSystem;

using SF.InputModule;

namespace SF.AbilityModule.Characters
{
    public class CrouchAbility : AbilityCore, IInputAbility
    {
        [SerializeField] private Vector2 _colliderResized = new Vector2(.5f,1.5f);

        private void OnInputCrouch(InputAction.CallbackContext context)
        {
            if (!CanStartAbility())
                return;
            
            //TODO: Need to do some collider check to make sure the character is not being pushed into a ceiling or something else when uncrouching.

            if(!CheckAbilityRequirements()) return;
            

            
            _controller2d.IsCrouching = !_controller2d.IsCrouching;

            // If statement acts like a toggle for crouching.
            if (_controller2d.IsCrouching)
            {
                _controller2d.ResizeCollider(_colliderResized);
                _isPerformingAbility = true;
            }
            else
            {
                _controller2d.ResetColliderSize();
                _isPerformingAbility = false;
            }
        }

        private void OnInputStopCrouching(InputAction.CallbackContext context)
        {
            // Have to make sure we are already crouching because we might cause issues
            // calling ResetColliderSize when are character doesn't need the collider size reset.
            if(!_controller2d.IsCrouching)
                return;
            
            // On Controllers sometimes the dead zone still registers a move axis when just ushing down on the controller.
            // If the value == (0,-1) the controllers are just slightly bumped sideways and this event doesn't actually need invoked.
            // context.valueSizeInBytes = 4 is a single value like space bar
            // context.valueSizeInBytes = 8 is like a Vector2 value.
            if (context.valueSizeInBytes >  4 && context.ReadValue<Vector2>() == Vector2.down)
                return;
            
            _controller2d.IsCrouching = false;
            _controller2d.ResetColliderSize();
        }

        protected override void OnAbilityInterruption()
        {
            _controller2d.IsCrouching = false;
            _controller2d.ResetColliderSize();
        }
        
        protected override bool CheckAbilityRequirements()
        {
            if (_controller2d.IsClimbing
                    || _controller2d.IsJumping
                    || _controller2d.IsFalling
                    || _controller2d.IsSwimming
                    || _controller2d.IsGliding
                    || !_controller2d.IsGrounded
                    || _controller2d.Direction.x != 0
                )
            {
                return false;
            }

            return true;
        }

        private void OnEnable()
        {
            InputManager.Controls.Player.Enable();
            InputManager.Controls.Player.Crouch.performed += OnInputCrouch;
            InputManager.Controls.Player.Move.performed += OnInputStopCrouching;
            InputManager.Controls.Player.Jump.performed += OnInputStopCrouching;
        }

        private void OnDisable()
        {
            if(InputManager.Controls == null) return;
            InputManager.Controls.Player.Crouch.performed -= OnInputCrouch;
            InputManager.Controls.Player.Move.performed -= OnInputStopCrouching;
            InputManager.Controls.Player.Jump.performed -= OnInputStopCrouching;
        }
    }
}
