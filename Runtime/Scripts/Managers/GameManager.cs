using System;
using System.Collections.Generic;
using UnityEngine;

namespace SF.Managers
{
    using DataManagement;
    using DialogueModule;
    using Settings;
	/// <summary>
	/// The current state that is controlling the games input and actions. 
	/// </summary>
	public enum GameControlState : uint 
	{
		Player = 0,
		SceneChanging = 1,
		Cutscenes = 2,
		Transition = 4, // Player being moved within a scene, but has no control over the player. Think teleporting.
        Dialogue = 8,
        Menu = 16,
	}
	/// <summary>
	/// The current play state of the game loop that describes what type of logic loop is being updated.
	/// </summary>

    [DefaultExecutionOrder(-5)]
    public class GameManager : ManagerBaseStaticCleanUp<GameManager>
    {
        [SerializeReference]
        public List<SaveDataBlock> SaveDataBlocks = new List<SaveDataBlock> ();

        [SerializeField] private GameControlState _controlState;
        
        public GameControlState ControlState
        {
            get { return _controlState;}
            set
            {
                if (_controlState != value)
                {
                    _controlState = value;
                    OnGameControlStateChanged?.Invoke(_controlState);
                }
            }
        }

        public Action<GameControlState> OnGameControlStateChanged;

        public static event Action GamePausedHandler;
        public static event Action GameUnpausedHandler;

        /// <summary>
        /// The data object that holds the settings players can change in game.
        /// </summary>
        public GameSettings GameSettings;
        
        protected override void Awake()
        {
            if(GameSettings != null)
                GameSettings.DisplaySettings.ProcessSettings();

            base.Awake();
        }
        
        protected void OnEnable()
        {
#if SF_DIALOGUE_GRAPH
            DialogueManager.DialogueStartedHandler += OnDialogueStarted;
            DialogueManager.DialogueEndedHandler += OnDialogueEnded;
#endif
        }

        protected void OnDisable ()
        {
#if SF_DIALOGUE_GRAPH
            DialogueManager.DialogueStartedHandler -= OnDialogueStarted;
            DialogueManager.DialogueEndedHandler -= OnDialogueEnded;
#endif
        }

        /// <summary>
        /// Exits the game and closes all related computer processes.
        /// </summary>
        public static void ExitGame()
        {
            // Will need to do checks later for preventing shutdowns during saving and loading.
            Application.Quit();
        }

        public static void OnPausedToggle()
        {
            if(Instance._controlState == GameControlState.Player)
                Pause();
            else // So we are already paused or in another menu.
                Unpause();
        }

        protected static void Pause()
        {
            Instance.ControlState = GameControlState.Menu;
            GamePausedHandler?.Invoke();
        }

        protected static void Unpause()
        {
            Instance.ControlState = GameControlState.Player;
            GameUnpausedHandler?.Invoke();
        }
        
        private void OnDialogueStarted()
        {
            /* TODO: Switch statement for type of dialogue.
            * Allow for background dialogue that don't freeze the player control. */
            
            ControlState = GameControlState.Dialogue;
        }
        
        private void OnDialogueEnded()
        {
            /* TODO: Switch statement for type of dialogue.
             * Allow for background dialogue that don't freeze the player control. */
            
            ControlState = GameControlState.Player;
        }
    }
}
