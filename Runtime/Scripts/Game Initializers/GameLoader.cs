using System;
using SF.DataManagement;
using SF.RoomModule;
using SF.ItemModule;
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
        
        /// <summary>
        /// This data object that keeps track of references needed to be loaded in playable levels before anything else.
        /// </summary>
        [SerializeField] private LevelPlayData _levelPlayData;

        public ItemDatabase ItemDatabase;
        
        /// <summary>
        /// This is run the first time the game is initialized in any scene.
        /// </summary>
        public static event Action GameInitializedHandler;
        
        private void Awake()
        {
            // The GameLoader will take care of all child game objects initialization.
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
        /// Called when the new game data has been set up and the first scene of the new game is completely loaded and initialized.
        /// </summary>
        private void OnNewGameReady()
        {
            if (GameLoaderData == null)
                return;
            
            GameLoaderData.SettingUpNewGame = false;
        }
    }
}
