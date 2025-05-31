using SF.CameraModule;
using SF.Managers;
using UnityEngine;


namespace SF.Characters.Controllers
{
	public class PlayerController : GroundedController2D
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            CameraController.Instance.CameraTarget = transform;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameControlStateChanged += OnGameControlStateChanged;
                if(GameManager.Instance.PlayerController == null)
                    GameManager.Instance.PlayerController = this;
            }
        }
        
        protected override void CalculateMovementState()
        {
            // For when in menu, in a conversation, and so forth.
            if (GameManager.Instance.ControlState != GameControlState.Player)
            {
                if (IsGrounded)
                {
                    CharacterState.CurrentMovementState = MovementState.Idle;
                    // Freeze the controller only after grounded so if we are stopped in mid-air we still hit the ground.
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