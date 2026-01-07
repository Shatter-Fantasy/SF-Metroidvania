using UnityEngine;
using UnityEngine.InputSystem;

using SF.InputModule;
using SF.AudioModule;

namespace SF.AbilityModule.Characters
{
    public class JumpAbility : AbilityCore, IInputAbility
    {
        [Header("Jumping Physics")]
        public float JumpHeight = 12;
        public float RunningJumpMultiplier = 1.1f;
        private float _calculatedJumpHeight;
        public int JumpAmount = 1;
        public int JumpsRemaining;

        public bool CanJumpInfinitely;

        [Header("Jumping SFX")]
        [SerializeField] private AudioClip _jumpSFX;

        protected override void OnInitialize()
        {
            _controller2d.CollisionInfo.OnGroundedHandler += ResetJumps;
        }

        /// <summary>
        /// Check if the character controller is in a movement state or if the ability has special logic checks to make sure it can be performed.
        /// <remarks>
        /// If you want to make sure the ability can start while checking the ControlState call <see cref="AbilityCore.CanStartAbility"/>.
        /// AbilityCore.CanStart calls the CheckAbilityRequirements in it as well so it will do both checks.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        protected override bool CheckAbilityRequirements()
        {
            /* If we are currently gliding don't jump. 
             * Do note we could add the ability to jump in mid-glide for more movement customization. Maybe a boolean in the Glide called CanGlideJump.
             * If we are touching a ceiling don't jump because it will be wierd to jump in place without moving upward animation wise. */
            if(_controller2d.IsGliding || _controller2d.CollisionInfo.IsCollidingAbove)
                return false;


            if(JumpsRemaining < 1 && !CanJumpInfinitely)
                return false;

            return true;
        }

        private void OnInputJump(InputAction.CallbackContext context)
		{
            if(!CanStartAbility()) return;
            // TODO: Only add the running height bonus to the first jump.
            _calculatedJumpHeight = _controller2d.IsRunning
                ? JumpHeight * RunningJumpMultiplier
                : JumpHeight;

            JumpsRemaining--;

			_controller2d.IsJumping = true;
            _controller2d.IsFalling = false;
            _controller2d.IsClimbing = false;

            if(_jumpSFX != null)
                AudioManager.Instance.PlayOneShot(_jumpSFX);
            
            _controller2d.SetVerticalVelocity(_calculatedJumpHeight);
		}

        public void ResetJumps()
        {
            JumpsRemaining = JumpAmount;
        }

        private void OnEnable()
		{
			SFInputManager.Controls.Player.Enable();
			SFInputManager.Controls.Player.Jump.performed += OnInputJump;
            
            // Have to check for null because you can have OnEnable run sometimes before initialization from the ability controller.
            if (_controller2d != null)
            { 
                _controller2d.CollisionInfo.OnGroundedHandler += ResetJumps;
            }
        }

        private void OnDisable()
		{
			if(SFInputManager.Controls == null) return;

			SFInputManager.Controls.Player.Jump.performed -= OnInputJump;
            
            if(_controller2d != null)
                _controller2d.CollisionInfo.OnGroundedHandler -= ResetJumps;
		}
    }
}
