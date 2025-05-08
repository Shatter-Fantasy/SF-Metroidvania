using SF.RoomModule;
using SF.DialogueModule;
using SF.LevelModule;
using UnityEngine;

namespace SF.Managers
{
    /// <summary>
    /// Keeps track of the prefabs, scriptable objects that need loaded for first scene (think RoomDB), and makes sure all required
    /// Managers/Databases are ready before needing to be used.
    /// </summary>
    [DefaultExecutionOrder(-5)]
    public class GameLoader : MonoBehaviour
    {
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
            if(Instance != null && Instance != this)
                Destroy(gameObject);

            /* Even after checking to make sure no other GameLoaders exists there could be one case the game was already initialized.
                The first GameLoader that initialized the GameManagers could have been destroyed/deloaded making Instance == null.
                Thus, we should also check if WasGameInitialized was set to true already in a different GameLoader InitializeGame call.
                */
            
            if (WasGameInitialized)
                return;
            
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
            
            InitializeInSceneManagers();
            
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
    }
}
