using SF.Weapons;
using UnityEngine;

namespace SF.Characters.Controllers
{
    using Managers;
    using U2D.Physics;
    /// <summary>
    /// A physics controller for the playable character that help implement gravity, slope mechanics, collision for platforms,
    /// and updates the <see cref="MovementState"/>.
    ///
    /// This player specific controller also implements logic for the game when  paused, character state is moved to a dialogue,
    /// and helps set up the instance object for other classes to know what is the player.
    /// <remarks>
    /// This sets up the PlayerController instance in the game manager during the awake call.
    /// In the start call for objects being loaded at the same time, other objects can now get a reference to
    /// the <see cref="PlayerController"/>.
    /// </remarks> 
    /// </summary>
	public class PlayerController : ControllerBody2D
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameControlStateChanged += OnGameControlStateChanged;

                if (!GameLoader.SettingUpNewGame)
                    CollisionInfo.CollisionActivated = true;
            }
        }
        
        protected override void CalculateHorizontal()
        {
            if(IsClimbing 
               || (CharacterState.AttackState == AttackState.Attacking && CollisionInfo.IsGrounded))
            {
                _calculatedVelocity.x = 0;
                return;
            }

            if(Direction.x != 0)
            {
                // We only have to do a single clamp because than Direction.x takes care of it being negative or not when being multiplied.
                ReferenceSpeed = Mathf.Clamp(ReferenceSpeed, 0, CurrentPhysics.GroundMaxSpeed);

                // TODO: When turning around erase previously directional velocity.
                // If it is kept the player could slide in the previous direction for a second before running the new direction on smaller ground acceleration values.
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, ReferenceSpeed * Direction.x, CurrentPhysics.GroundAcceleration);
            }
            else
            {
                _calculatedVelocity.x = Mathf.MoveTowards(_calculatedVelocity.x, 0, CurrentPhysics.GroundDeacceleration);
            }
			
            // If we are moving left and not hitting a slope, but an obstacle, stop moving left.
            if (CollisionInfo.IsCollidingLeft && Direction.x < 0 && CollisionInfo.IsCollidingLeft)
            {
                _calculatedVelocity.x = 0;
            }
			
            // If we are moving Right and not hitting a slope, but an obstacle, stop moving Right.
            if (CollisionInfo.IsCollidingRight && Direction.x > 0 && CollisionInfo.IsCollidingRight)
            {
                _calculatedVelocity.x = 0;
            }
        }

        protected override void CalculateMovementState()
        {
            // For when in menu, in a conversation, and so forth.
            if (GameManager.Instance?.ControlState != GameControlState.Player)
            {
                if (CollisionInfo.IsGrounded)
                {
                    CharacterState.CurrentMovementState = MovementState.Idle;
                    // Freeze the controller only after grounded so if we are stopped in midair we still hit the ground.
                }

                return;
            }
            
            base.CalculateMovementState();
        }

        private void OnGameControlStateChanged(GameControlState controlState)
        {
            // If we are exiting dialogue or a menu unfreeze the player.
            if(controlState == GameControlState.Player)
                UnfreezeController();
            else
            {
                FreezeController();
            }
        }
    }
}