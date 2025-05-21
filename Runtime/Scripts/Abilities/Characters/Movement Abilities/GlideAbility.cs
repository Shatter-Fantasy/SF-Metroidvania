using System;
using UnityEngine;
using UnityEngine.InputSystem;

using SF.InputModule;
using SF.Physics;

namespace SF.AbilityModule.Characters
{
    public class GlideAbility : AbilityCore, IInputAbility
    {
        [Header("Gravity Physics")]

        [SerializeField] private MovementProperties DefaultGlideProperties;

        protected override void OnInitialize()
        {
           _controller2d.OnGrounded += GlideReset;
        }

        protected override bool CheckAbilityRequirements()
        {
            if(!_isInitialized || !enabled || _controller2d == null)
                return false;

            if(_controller2d.PhysicsVolumeType == PhysicsVolumeType.Water)
                return false;

            // If we are grounded we don't need to glide.
            if(_controller2d.IsGrounded)
                return false;
            return true;
        }
        private void OnInputGlide(InputAction.CallbackContext context)
        {
            Debug.Log(context.ReadValue<Single>());
            if(!CheckAbilityRequirements()) return;

            _controller2d.SetVerticalVelocity(0);
            _controller2d.UpdatePhysics(DefaultGlideProperties);
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
            InputManager.Controls.Player.Enable();
            InputManager.Controls.Player.Glide.performed += OnInputGlide;
            InputManager.Controls.Player.Jump.performed += OnMidGlideJump;
        }

        private void OnDisable()
        {
            if(InputManager.Controls == null) return;

            InputManager.Controls.Player.Glide.performed -= OnInputGlide;
            InputManager.Controls.Player.Jump.performed -= OnMidGlideJump;
            _controller2d.OnGrounded -= GlideReset;
        }
    }
}
