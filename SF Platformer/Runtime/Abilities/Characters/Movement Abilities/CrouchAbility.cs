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
            if (!CanStartAbility())
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
