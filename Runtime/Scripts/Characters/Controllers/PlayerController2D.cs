using SF.LevelModule;
using SF.Managers;

namespace SF.Characters.Controllers
{
    /// <summary>
    /// A physics controller for the playable character that help implement gravity, slope mechanics, collision for platforms,
    /// and updates the <see cref="MovementState"/> while using the <see cref="SF.Physics.CollisionController"/> for custom collision callbacks.
    ///
    /// This player specific controller also implements logic for the game when  paused, character state is moved to a dialogue,
    /// and helps set up the instance object for other classes to know what is the player.
    /// <remarks>
    /// This sets up the PlayerController instance in the game manager during the awake call.
    /// In the start call for objects being loaded at the same time, other objects can now get a reference to
    /// the <see cref="PlayerController"/> after it is set in <see cref="PlayerController.OnAwake"/>.
    /// </remarks> 
    /// </summary>
	public class PlayerController : GroundedController2D
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameControlStateChanged += OnGameControlStateChanged;
                if(LevelPlayData.Instance.SpawnedPlayerController == null)
                    LevelPlayData.Instance.SpawnedPlayerController = this;

                if (!GameLoader.Instance.GameLoaderData.SettingUpNewGame)
                    CollisionInfo.CollisionActivated = true;
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