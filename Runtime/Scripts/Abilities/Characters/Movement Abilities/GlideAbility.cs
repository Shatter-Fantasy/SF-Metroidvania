using UnityEngine;
using UnityEngine.InputSystem;

using SF.InputModule;
using SF.PhysicsLowLevel;

namespace SF.AbilityModule.Characters
{
    /// <summary>
    /// Grants the ability for a player to Glide with a set input command.
    /// Controller by an <see cref="AbilityController"/>.
    /// </summary>
    public class GlideAbility : AbilityCore, IInputAbility
    {
        [Header("Gravity Physics")]

        [SerializeField] private MovementProperties DefaultGlideProperties;

        protected override void OnInitialize()
        {
            _controller2d.CollisionInfo.OnGroundedHandler += GlideReset;
        }

        protected override bool CheckAbilityRequirements()
        {
            if(!_isInitialized || !enabled || _controller2d == null)
                return false;

            if(_controller2d.PhysicsVolumeType == PhysicsVolumeType.Water)
                return false;

            // If we are grounded we don't need to glide.
            if(_controller2d.CollisionInfo.IsGrounded)
                return false;
            return true;
        }
        private void OnInputGlide(InputAction.CallbackContext context)
        {
            if(!CheckAbilityRequirements()) return;

            _controller2d.SetVerticalVelocity(0);
            _controller2d.UpdatePhysicsProperties(DefaultGlideProperties);
            _controller2d.IsGliding = true;
        }

        private void GlideReset()
        {
            if(!_controller2d.IsGliding)
                return;

            _controller2d.IsGliding = false;
            _controller2d.ResetPhysics(_controller2d.DefaultPhysics);
        }

        private void OnMidGlideJump(InputAction.CallbackContext context)
        {
            GlideReset();
        }

        private void OnEnable()
        {
            SFInputManager.Controls.Player.Enable();
            SFInputManager.Controls.Player.Glide.performed += OnInputGlide;
            SFInputManager.Controls.Player.Jump.performed += OnMidGlideJump;
        }

        private void OnDisable()
        {
            if(SFInputManager.Controls == null) return;

            SFInputManager.Controls.Player.Glide.performed -= OnInputGlide;
            SFInputManager.Controls.Player.Jump.performed -= OnMidGlideJump;
            _controller2d.CollisionInfo.OnGroundedHandler-= GlideReset;
        }
    }
}
