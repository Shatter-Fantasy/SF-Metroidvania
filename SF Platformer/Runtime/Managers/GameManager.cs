using System;

using SF.Characters.Controllers;
using SF.DataManagement;
using SF.Events;
using SF.InputModule;
using SF.Utilities.Shapes;

using UnityEngine;
using UnityEngine.InputSystem;

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
		TransformTransition // Player being moved within a scene, but has no control over the player. Think teleporting.
	}

	/// <summary>
	/// The current play state of the game loop that describes what type of logic loop is being updated.
	/// </summary>
	public enum GamePlayState
	{
		Playing = 0,
		Paused = 1,
		MainMenu = 2
	}

    [DefaultExecutionOrder(-5)]
    [RequireComponent(typeof(LivesManager))]
    public class GameManager : MonoBehaviour, EventListener<ApplicationEvent>, EventListener<GameEvent>
    {
        [SerializeField] private int _targetFrameRate = 60;
		public GameControlState ControlState;
		public GamePlayState PlayState;

        private static GameObject PlayerSceneObject;

        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
        private void Start()
        {
            PlayerSceneObject = FindAnyObjectByType<PlayerController>().gameObject;
            SaveSystem.LoadDataFile();
        }
        private void OnExitGame()
        {
            // Will need to do checks later for preventing shutdowns during saving and loading.
            Application.Quit();
        }

        private void OnPausedToggle()
        {
            if(PlayState == GamePlayState.Playing)
                Pause();
            else // So we are already paused or in another menu.
                Unpause();
        }

        private void Pause()
        {
            PlayState = GamePlayState.MainMenu;
            GameMenuEvent.Trigger(GameMenuEventTypes.OpenGameMenu);
        }

        private void Unpause()
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

        private void OnEnable()
		{
            this.EventStartListening<ApplicationEvent>();
            this.EventStartListening<GameEvent>();
		}

        private void OnDisable ()
		{
            this.EventStopListening<ApplicationEvent>();
            this.EventStopListening<GameEvent>();
        }

    }
}
