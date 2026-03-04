using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.Managers
{
    using DataManagement;
    using RoomModule;
    using ItemModule;
    
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
            
            GameInitializedHandler?.Invoke();
            WasGameInitialized = true;
        }

        /// <summary>
        /// Sets up the base state of a new game.
        /// </summary>
        public void NewGame()
        {
            MetroidvaniaSaveManager.StartingRoom = RoomSystem.RoomDB != null 
                ? RoomSystem.RoomDB.StartingRoomID 
                : 0;
            
            if (GameLoaderData == null)
                return;

            GameLoaderData.SettingUpNewGame = true;
            SceneManager.LoadScene(GameLoaderData.NewGameSceneIndex);
        }

        public void LoadGame()
        {
            MetroidvaniaSaveManager.StartingRoom = RoomSystem.RoomDB != null 
                ? RoomSystem.RoomDB.StartingRoomID 
                : 0;
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
