using SF.AbilityModule;
using SF.Events;
using SF.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SF.InputModule
{
    public class InputManager : MonoBehaviour
    {
	    
	    /// <summary>
	    /// Used to keep track of all abilities that implement <see cref="IInputAbility"/> on the player.
	    /// Allows the ability to set new input events to the player abilities when needed.
	    /// </summary>
	    private AbilityController _playerAbilityController;
	    
		private static InputManager _instance;
		public static InputManager Instance
		{
			get
			{
				if(_instance == null)
					_instance = FindAnyObjectByType<InputManager>();

				return _instance;
			}
		}

		private static Controls _controls;
		public static Controls Controls 
		{
			get
			{
				_controls ??= new Controls();

				return _controls;
			}
		}
		
		private void Awake()
		{
			if(Instance != null && Instance != this)
				Destroy(this);
		}

		private void Start()
		{
			if (GameManager.Instance != null)
				GameManager.Instance.OnGameControlStateChanged += OnGameControlStateChanged;
		}
		
		private void OnGameMenuToggled(InputAction.CallbackContext ctx)
		{
			GameEvent.Trigger(GameEventTypes.PauseToggle);
		}

		public void EnableActionMap()
        {

        }

        public void DisableActionMap()
        {

        }
        
        private void OnGameControlStateChanged(GameControlState controlState)
        {
	        // If we are exiting dialogue or a menu unfreeze the player.
	        if (controlState == GameControlState.Player)
	        {
		        Controls.Player.Enable();
		        Controls.UI.Disable();
	        }
	        else
	        {
		        Controls.UI.Enable();
		        Controls.Player.Disable();
	        }
        }
        
        private void OnEnable()
        {
			if(Controls != null)
			{
				Controls.GameControl.Enable();
				Controls.GameControl.PauseToggle.performed += OnGameMenuToggled;
			}
        }
        private void OnDisable()
        {
			if(Controls != null)
			{
				Controls.GameControl.PauseToggle.performed -= OnGameMenuToggled;
			}
        }

        private void OnDestroy()
        {
	        if (Controls != null)
	        {
		        Controls.Player.Disable();
		        Controls.UI.Disable();
		        Controls.GameControl.Disable();
	        }
        }
    }
}