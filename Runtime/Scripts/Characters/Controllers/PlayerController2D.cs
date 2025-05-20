using SF.CameraModule;
using SF.Managers;


namespace SF.Characters.Controllers
{
	public class PlayerController : GroundedController2D
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            CameraController.Instance.CameraTarget = transform;

            if (GameManager.Instance != null && GameManager.Instance.PlayerController == null)
                GameManager.Instance.PlayerController = this;
        }
        
        protected override void CalculateMovementState()
        {
            // For when in menu, in a conversation, and so forth.
            if (GameManager.Instance.ControlState != GameControlState.Player)
            {
                if (IsGrounded)
                    CharacterState.CurrentMovementState = MovementState.Idle;
                
                FreezeController();
                return;
            }
            
            base.CalculateMovementState();
        }
    }
}