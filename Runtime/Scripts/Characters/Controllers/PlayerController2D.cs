using SF.Managers;
using SF.PhysicsLowLevel;

namespace SF.Characters.Controllers
{
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
                //if(LevelPlayData.Instance.spawnedPlayerController == null)
                  //  LevelPlayData.Instance.spawnedPlayerController = this;

                if (!GameLoader.Instance.GameLoaderData.SettingUpNewGame)
                    CollisionInfo.CollisionActivated = true;
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