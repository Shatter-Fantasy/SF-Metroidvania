using System;
using SF.DataManagement;
using SF.RoomModule;
using SF.DialogueModule;
using SF.LevelModule;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.Managers
{
    /// <summary>
    /// Keeps track of the prefabs, scriptable objects that need loaded for first scene (think RoomDB), and makes sure all required
    /// Managers/Databases are ready before needing to be used.
    /// </summary>
    [DefaultExecutionOrder(-5)]
    public class GameLoader : MonoBehaviour
    {
        [SerializeField] private GameLoaderSO _gameLoaderData; 
        
        public static GameLoader Instance;
        public static bool WasGameInitialized = false;
        /* Since Scriptable Objects don't have their lifecycle events done until they are referenced in scene,
         we set them up via the GameLoader Scriptable Object with a RuntimeInitializeOnLoadMethod
         which set the values of the GameManager on first scene load. */
        [Header("Required Databases DB ")]
        [SerializeField] private RoomDB _roomDB;

        [SerializeField] private DialogueDatabase _dialogueDatabase;
        /// <summary>
        /// This data object that keeps track of references needed to be loaded in playable levels before anything else.
        /// </summary>
        [SerializeField] private LevelPlayData _levelPlayData;

        /// <summary>
        /// The prefab that acts as the root game object for all game wide managers.
        /// </summary>
        /// <remarks>
        /// Think managers in all scenes not just the scenes for playing the game.
        /// Example non-play scenes needing managers loaded - starting menu scene.
        /// Think of managers like Audio/Input.
        /// </remarks>
        [Header("Required Managers")] 
        [SerializeField] private GameObject _gameWideManagers;
        
        private void Awake()
        {
            // The GameLoader will take care of all child gameobjects initialization.
            // If one was already set and initialized do not reinit and load duplicate game managers.
            // Destroy this entire GameObject to prevent duplicate managers.
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;

            if (_gameLoaderData != null)
                SceneManager.sceneLoaded += OnSceneLoaded;
            
            DontDestroyOnLoad(this);

            /* Even after checking to make sure no other GameLoaders exists there could be one case the game was already initialized.
                The first GameLoader that initialized the GameManagers could have been destroyed/deloaded making Instance == null.
                Thus, we should also check if WasGameInitialized was set to true already in a different GameLoader InitializeGame call.
                */
            
            if (WasGameInitialized)
                return;

            if (_roomDB != null)
                RoomDB.Instance = _roomDB;
            
            InitializeGame();
        }

        /// <summary>
        ///  Initializes the entire game including the game managers that exist in scene,
        ///     the scriptable objects needing to be set up before used (example RoomDB),
        ///     and game settings like graphics/audio.
        /// </summary>
        public void InitializeGame()
        {
            if (_levelPlayData != null)
                LevelPlayData.Instance = _levelPlayData;
            
            //InitializeInSceneManagers();
            
            WasGameInitialized = true;
        }
        
        /// <summary>
        /// Initializes the managers for the game that exist inside the scene view.
        /// </summary>
        public void InitializeInSceneManagers()
        {
            if (_gameWideManagers == null)
            {
                Debug.Log("There was no prefab for GameWideManagers assigned in the GameLoader component", gameObject);
                return;
            }
        }

        /// <summary>
        /// Sets up the base state of a new game.
        /// </summary>
        public void NewGame()
        {
            // Set the starting room first.
            if (_gameLoaderData == null)
                return;

            _gameLoaderData.SettingUpNewGame = true;
            MetroidvaniaSaveManager.StartingRoom = _gameLoaderData.StartingRoomID;

            SceneManager.LoadScene(_gameLoaderData.NewGameSceneIndex);
        }

        /// <summary>
        /// Called when the new game data has been set up and the first scene of the new game is completely loaded and initialized.
        /// </summary>
        private void OnNewGameReady()
        {
            if (_gameLoaderData == null)
                return;
            
            _gameLoaderData.SettingUpNewGame = false;
        }
        public void LoadGame()
        {
            // Set the starting room first.
            if(_gameLoaderData != null)
                MetroidvaniaSaveManager.StartingRoom = _gameLoaderData.StartingRoomID;
        }
        
        /// <summary>
        /// /Called by the SceneManager when any scene is loaded.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (_gameLoaderData == null)
                return;
            
            // When we are loading the same scene as a new game also check if we are in the middle of setting a new game file.
            if (scene.buildIndex == _gameLoaderData.NewGameSceneIndex && _gameLoaderData.SettingUpNewGame)
                NewGameSceneInitialization();
        }
        
        /// <summary>
        /// This is called when the new game scene is finished loaded.
        /// <remarks>
        /// This should be called after all the starting data has been assigned in the <see cref="GameLoader"/> such as the databases and starting room.
        /// </remarks> 
        /// </summary>
        private void NewGameSceneInitialization()
        {
            Debug.Log("A new game is being set up.");

            OnNewGameReady();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
