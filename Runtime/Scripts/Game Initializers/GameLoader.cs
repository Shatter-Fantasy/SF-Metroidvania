using System;
using SF.CameraModule;
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
        [field: SerializeField] public GameLoaderSO GameLoaderData { get; private set; }

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
        
        private void Awake()
        {
            // The GameLoader will take care of all child gameobjects initialization.
            // If one was already set and initialized do not reinit and load duplicate game managers.
            // Destroy this entire GameObject to prevent duplicate managers.
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
            
            InitializeGame();
        }

        /// <summary>
        ///  Initializes the entire game including the game managers that exist in scene,
        ///     the scriptable objects needing to be set up before used (example RoomDB),
        ///     and game settings like graphics/audio.
        /// </summary>
        public void InitializeGame()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this);
            
            /* Even after checking to make sure no other GameLoaders exists there could be one case the game was already initialized.
            The first GameLoader that initialized the GameManagers could have been destroyed/deloaded making Instance == null.
            Thus, we should also check if WasGameInitialized was set to true already in a different GameLoader InitializeGame call.*/
            if (WasGameInitialized)
                return;
            
            if (_roomDB != null)
                RoomSystem.RoomDB = _roomDB;
            
            if (_levelPlayData != null)
                LevelPlayData.Instance = _levelPlayData;
            
            WasGameInitialized = true;
        }

        /// <summary>
        /// Sets up the base state of a new game.
        /// </summary>
        public void NewGame()
        {
            if (GameLoaderData == null)
                return;

            GameLoaderData.SettingUpNewGame = true;
            MetroidvaniaSaveManager.StartingRoom = GameLoaderData.StartingRoomID;
            SceneManager.LoadScene(GameLoaderData.NewGameSceneIndex);
        }

        public void LoadGame()
        {
            // Set the starting room first.
            if(GameLoaderData != null)
                MetroidvaniaSaveManager.StartingRoom = GameLoaderData.StartingRoomID;
        }
        
        /// <summary>
        /// /Called by the SceneManager when any scene is loaded.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (GameLoaderData == null)
                return;
            
            // We will be doing stuff here later so the NewGameSceneInitialization is staying in a separate function for now 
            
            // When we are loading the same scene as a new game also check if we are in the middle of setting a new game file.
            if (scene.buildIndex == GameLoaderData.NewGameSceneIndex && GameLoaderData.SettingUpNewGame)
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
            /* Keep in mind the OnSceneLoaded callback that will call this function will be called after
                awake has run on the gameobject in the scene already as it is loaded.
                
                Example: The RoomControllers that exist in the scene already if the rooms were not dynamically loaded
                will have already set their spawned instances and the linked room in the room database initialized.*/
            
            RoomSystem.SetInitialRoom(GameLoaderData.StartingRoomID);
            
            // TODO: Create the proper spawn system for loading in game or any play spawn positioning.
            //  The if statement and stuff in it should be remove sooner or later
            if (_levelPlayData.SpawnedPlayerController != null && CameraController.ActiveRoomCamera != null)
            {
                Vector2 startingPosition = CameraController.ActiveRoomCamera.transform.position;
                _levelPlayData.SpawnedPlayerController.transform.position = new Vector3(startingPosition.x,startingPosition.y,0);
            }

            OnNewGameReady();
        }

        
        /// <summary>
        /// Called when the new game data has been set up and the first scene of the new game is completely loaded and initialized.
        /// </summary>
        private void OnNewGameReady()
        {
            if (GameLoaderData == null)
                return;
            
            GameLoaderData.SettingUpNewGame = false;
            LevelPlayData.Instance.SpawnedPlayerController.CollisionActivated = true;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
