using System;
using UnityEngine;

namespace SF.Managers
{
    using RoomModule;
    using SF.DataManagement;
    public enum GameLoadingMode
    {
        NewGame, LoadGame, Continue
    }
    
    /// <summary>
    /// Declares the default order for important game features other than physics.
    /// For Physics DefaultExecutionOrder see <see cref="SF.U2D.Physics.PhysicsCore2DExecutionOrder"/>
    /// </summary>
    public static class GameDefaultExecutionOrders
    {
        public const int DatabaseExecutionOrder = -10;
    }
    
    /// <summary>
    /// Keeps track of the prefabs, scriptable objects that need loaded for first scene (think RoomDB), and makes sure all required
    /// Managers/Databases are ready before needing to be used.
    /// </summary>
    [DefaultExecutionOrder(-5)]
    public class GameLoader : ManagerBaseStaticCleanUp<GameLoader>
    {
        
        /// <summary>
        /// The index of the scene for a new game inside the build profile list.
        /// </summary>
        [Header("Scene Loading Data")]
        [field: SerializeField] public int NewGameSceneIndex { get; private set; } = 1;
        
        public static bool WasGameInitialized = false;
        /// <summary>
        /// Is set to true when a new game is being initialized.
        /// <remarks>
        /// This is used in places like SceneManager.loadedScene event callbacks to see if we are loading a new game first playable scene or not.
        /// </remarks>
        /// </summary>
        public static bool SettingUpNewGame;
        
        /// <summary>
        /// This is run the first time the game is initialized in any scene.
        /// </summary>
        public static event Action GameInitializedHandler;
        
        protected override void Awake()
        {
            base.Awake();
            
            // The GameLoader will take care of all child game objects initialization.
            // If one was already set and initialized do not reinit and load duplicate game managers.
            // Destroy this entire GameObject to prevent duplicate managers.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
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
            MetroidvaniaSaveManager.NewGame();
        }

        public void LoadGame()
        {
            // TODO: Implement the file loading and get the last saved rooms room id and region.
            RoomSystem.StartingRoomId = 0;
        }
        
        /// <summary>
        /// Called when the new game data has been set up and the first scene of the new game is completely loaded and initialized.
        /// </summary>
        private void OnNewGameReady()
        {
            SettingUpNewGame = false;
        }
    }
}
