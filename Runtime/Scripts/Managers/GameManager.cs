using System;
using System.Collections.Generic;
using SF.DataManagement;
using SF.DialogueModule;
using SF.Events;
using UnityEngine;

namespace SF.Managers
{
	/// <summary>
	/// The current state that is controlling the games input and actions. 
	/// </summary>
	public enum GameControlState
	{
		Player,
		SceneChanging,
		Cutscenes,
		Transition, // Player being moved within a scene, but has no control over the player. Think teleporting.
        Dialogue,
        Menu,
	}
	/// <summary>
	/// The current play state of the game loop that describes what type of logic loop is being updated.
	/// </summary>

    [DefaultExecutionOrder(-5)]
    public class GameManager : MonoBehaviour, EventListener<ApplicationEvent>, EventListener<GameEvent>
    {
        [SerializeReference]
        public List<SaveDataBlock> SaveDataBlocks = new List<SaveDataBlock> ();
        
        [SerializeField] protected int _targetFrameRate = 60;
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
        
        public static GameManager Instance;

        public Action<GameControlState> OnGameControlStateChanged;
        
        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;

            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
                Destroy(gameObject); // We want to destroy the child object managers so they are not doubles as well.
        }
        
        protected void OnEnable()
        {
            this.EventStartListening<ApplicationEvent>();
            this.EventStartListening<GameEvent>();
            DialogueManager.DialogueStartedHandler += OnDialogueStarted;
            DialogueManager.DialogueEndedHandler += OnDialogueEnded;
        }

        protected void OnDisable ()
        {
            this.EventStopListening<ApplicationEvent>();
            this.EventStopListening<GameEvent>();
            DialogueManager.DialogueStartedHandler -= OnDialogueStarted;
            DialogueManager.DialogueEndedHandler -= OnDialogueEnded;
        }

        protected void OnExitGame()
        {
            // Will need to do checks later for preventing shutdowns during saving and loading.
            Application.Quit();
        }

        protected void OnPausedToggle()
        {
            if(_controlState == GameControlState.Player)
                Pause();
            else // So we are already paused or in another menu.
                Unpause();
        }

        protected void Pause()
        {
            _controlState = GameControlState.Menu;
            GameMenuEvent.Trigger(GameMenuEventTypes.OpenGameMenu);
        }

        protected void Unpause()
        {
            _controlState = GameControlState.Player;
            GameMenuEvent.Trigger(GameMenuEventTypes.CloseGameMenu);
        }

        public void OnEvent(ApplicationEvent eventType)
        {
            switch(eventType.EventType)
            {
                case ApplicationEventTypes.ExitApplication:
                    {
                        OnExitGame();
                        break;
                    }
            }
        }

        public void OnEvent(GameEvent eventType)
        {
            switch(eventType.EventType)
            {
                case GameEventTypes.PauseToggle:
                    {
                        OnPausedToggle();
                        break;
                    }
            }
        }
        
        private void OnDialogueStarted()
        {
            /* TODO: Switch statement for type of dialogue.
            * Allow for background dialogue that don't freeze the player control. */
            _controlState = GameControlState.Dialogue;
        }
        
        private void OnDialogueEnded()
        {
            /* TODO: Switch statement for type of dialogue.
             * Allow for background dialogue that don't freeze the player control. */
            _controlState = GameControlState.Player;
        }
    }
}
