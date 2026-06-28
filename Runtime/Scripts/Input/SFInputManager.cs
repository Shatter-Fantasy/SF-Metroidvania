using UnityEngine.InputSystem;

namespace SF.InputModule
{
	using AbilityModule;
	using Managers;
	
    public class SFInputManager : ManagerBaseStaticCleanUp<SFInputManager>
    {
	    
	    /// <summary>
	    /// Used to keep track of all abilities that implement <see cref="IInputAbility"/> on the player.
	    /// Allows the ability to set new input events to the player abilities when needed.
	    /// </summary>
	    private AbilityController _playerAbilityController;

		private static Controls _controls;
		public static Controls Controls 
		{
			get
			{
				_controls ??= new Controls();

				return _controls;
			}
		}
		
		protected override void Awake()
		{
			base.Awake();
			if (Instance != null && Instance != this)
				Destroy(this);
		}

		private void Start()
		{
			if (GameManager.Instance != null)
				GameManager.Instance.OnGameControlStateChanged += OnGameControlStateChanged;
		}
		
		private void OnGameMenuToggled(InputAction.CallbackContext ctx)
		{
			GameManager.OnPausedToggle();
		}

		public void EnableActionMap()
        {

        }

        public void DisableActionMap()
        {

        }
        
        private void OnGameControlStateChanged(GameControlState controlState)
        {

	        switch (controlState)
	        {
		        case GameControlState.Player:
		        {
			        Controls.Player.Enable();
			        Controls.UI.Disable();
			        break;
		        }
		        case GameControlState.Dialogue:
		        case GameControlState.Menu:
		        {
			        Controls.UI.Enable();
			        Controls.Player.Disable();
			        break;
		        }
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
			// Without this when loading a new scene with an SFInputManager in it the new one will disable the Controls.
			if(Instance != this)
				return;
			
	        if (Controls != null)
	        {
		        Controls.Player.Disable();
		        Controls.UI.Disable();
		        Controls.GameControl.Disable();
	        }
        }
    }
}