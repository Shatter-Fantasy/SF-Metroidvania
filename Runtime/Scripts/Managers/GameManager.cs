using System;
using System.Collections.Generic;
using SF.Characters.Controllers;
using SF.DataManagement;
using SF.DialogueModule;
using SF.Events;
using SF.RoomModule;
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
		CameraTransition,
		TransformTransition, // Player being moved within a scene, but has no control over the player. Think teleporting.
        Dialogue
	}

	/// <summary>
	/// The current play state of the game loop that describes what type of logic loop is being updated.
	/// </summary>
	public enum GamePlayState
	{
		Playing = 0,
		Paused = 1,
		MainMenu = 2,
	}

    [DefaultExecutionOrder(-5)]
    public class GameManager : MonoBehaviour, EventListener<ApplicationEvent>, EventListener<GameEvent>, EventListener<DialogueEvent>
    {
        [SerializeReference]
        public List<SaveDataBlock> SaveDataBlocks = new List<SaveDataBlock> ();
        
        #if UNITY_EDITOR
        /// <summary>
        /// When playing from editor you can set a default room to load into to bypass the starting spawn room.
        /// </summary>
        [Header("Editor only debug settings.")][SerializeField] private bool _loadDebugRoom = false;
        /// <summary>
        /// The <see cref="Room.RoomID"/> of a debug room to start in.
        /// </summary>
        [SerializeField] private int _debugRoomID;
        
        /// <summary>
        /// Set false to not load a save file allowing the player to spawn in place for debugging in the editor.
        /// </summary>
        [SerializeField] protected bool _shouldLoadData;
        #endif
        
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
		public GamePlayState PlayState;
        
        public static GameManager Instance;

        public PlayerController PlayerController;

        public Action<GameControlState> OnGameControlStateChanged;
        
        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;

            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }
        
        public void InitializeLoadedGame()
        {
            
#if UNITY_EDITOR
            if (_shouldLoadData)
            {
#endif
                MetroidvaniaSaveManager.LoadGame();
                SaveDataBlocks = SaveSystem.CurrentSaveDataBlocks();
#if UNITY_EDITOR
            }

            if (_loadDebugRoom)
            {
                RoomSystem.SetInitialRoom(_debugRoomID);
            }
#endif
        }

        
        protected void OnExitGame()
        {
            // Will need to do checks later for preventing shutdowns during saving and loading.
            Application.Quit();
        }

        protected void OnPausedToggle()
        {
            if(PlayState == GamePlayState.Playing)
                Pause();
            else // So we are already paused or in another menu.
                Unpause();
        }

        protected void Pause()
        {
            PlayState = GamePlayState.MainMenu;
            GameMenuEvent.Trigger(GameMenuEventTypes.OpenGameMenu);
        }

        protected void Unpause()
        {
            PlayState = GamePlayState.Playing;
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
        
        public void OnEvent(DialogueEvent eventType)
        {
            switch (eventType.EventType)
            {
                case DialogueEventTypes.DialogueOpen:
                {
                    ControlState = GameControlState.Dialogue;
                    break;
                }
                case DialogueEventTypes.DialogueClose:
                {
                    ControlState = GameControlState.Player;
                    break;
                }
            }
        }
        
        protected void OnEnable()
		{
            this.EventStartListening<ApplicationEvent>();
            this.EventStartListening<GameEvent>();
            this.EventStartListening<DialogueEvent>();
		}

        protected void OnDisable ()
		{
            this.EventStopListening<ApplicationEvent>();
            this.EventStopListening<GameEvent>();
            this.EventStopListening<DialogueEvent>();
        }
    }
}
