using SF.Characters.Controllers;
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
        [SerializeField] protected int _targetFrameRate = 60;
		public GameControlState ControlState;
		public GamePlayState PlayState;

        protected static GameObject PlayerSceneObject;
        public static GameManager Instance;

        public PlayerController PlayerController;
        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;

            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }
        protected virtual void Start()
        {
            PlayerSceneObject = FindAnyObjectByType<PlayerController>().gameObject;
            SaveSystem.LoadDataFile();
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
