using System;
using SF.DataManagement;
using SF.RoomModule;
using SF.DialogueModule;
using SF.Inventory;
using SF.LevelModule;
using SF.SpawnModule;
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

        public ItemDatabase ItemDatabase;

        /// <summary>
        /// These are the indexes that are considered not part of a playable game scene.
        /// Any scene that has an index in this array will not start the First Playable Scene Loaded sequence.
        /// </summary>
        [Header("Scene Initialization")] [SerializeField]
        private int[] _gameStartingSceneIndexes = new int[1];
        
        /// <summary>
        /// This is run the first time the game is initialized in any scene.
        /// </summary>
        public static event Action GameInitializedHandler;

        /// <summary>
        /// This is called when the first playable is ready to give the player control.
        /// </summary>
        public static event Action LevelReadyHandler;

        private void Awake()
        {
            // The GameLoader will take care of all child game objects initialization.
            // If one was already set and initialized do not reinit and load duplicate game managers.
            // Destroy this entire GameObject to prevent duplicate managers.
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
            
            // This has to be done in awake. OnEnable/Start is called after the first sceneLoaded call.
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            InitializeGame();
        }

        /// <summary>
        ///  Initializes the entire game including the game managers that exist in scene,
        ///     the scriptable objects needing to be set up before used (example RoomDB),
        ///     and game settings like graphics/audio.
        /// </summary>
        public void InitializeGame()
        {
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
            
            GameInitializedHandler?.Invoke();
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
            for (int i = 0; i < _gameStartingSceneIndexes.Length; i++)
            {
                // If this is one of the game starting menu scenes don't bother running the playable scene initialization loop.
                if (_gameStartingSceneIndexes[i] == scene.buildIndex)
                {
                    return;
                }
            }

            PlayableGameSceneInitialization();
        }

        private void PlayableGameSceneInitialization()
        {
            /* Keep in mind the OnSceneLoaded callback that will call this function will be called after
                awake has run on the gameobject in the scene already as it is loaded.

                Example: The RoomControllers that exist in the scene already, if the rooms were not dynamically loaded,
                will have already set their spawned instances and the linked room in the room database initialized. */
            
            RoomSystem.SetInitialRoom(GameLoaderData.StartingRoomID);
            SpawnSystem.OnInitialPlayerSpawn(_levelPlayData.PlayerPrefab.gameObject);
            LevelReadyHandler?.Invoke();
        }

        
        /// <summary>
        /// Called when the new game data has been set up and the first scene of the new game is completely loaded and initialized.
        /// </summary>
        private void OnNewGameReady()
        {
            if (GameLoaderData == null)
                return;
            
            GameLoaderData.SettingUpNewGame = false;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
