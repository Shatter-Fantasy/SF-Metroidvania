using SF.Characters.Controllers;
using UnityEngine;

namespace SF.LevelModule
{
    /// <summary>
    /// Data object that contains data that needs to be referenced by objects in a scene that is a playable level.
    /// This object is only guaranteed valid in scenes where the playable character is being controlled.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelPlayData", menuName = "SF/Initializers/Level Play Data")]
    public class LevelPlayData : ScriptableObject
    {
        /// <summary>
        /// This is the player prefab asset that should be spawned in playable scenes.
        /// After this is spawned you use the <see cref="SpawnedPlayerController"/> value.
        /// </summary>
        [SerializeField] private PlayerController _playerPrefab;

        private PlayerController _spawnedPlayerController;
        
        /// <summary>
        /// The spawned <see cref="PlayerController"/> inside the current loaded scene.
        /// </summary>
        /// <remarks>
        /// This is only assigned right before the Awake/OnEnable calls of the first playable character scene is loaded.
        /// First set of menus do will not be able to use this.
        /// </remarks>
        public PlayerController SpawnedPlayerController
        {
            get
            {
                if (_spawnedPlayerController == null)
                {
                    // If none is already assigned make sure we don't have one in the scene for debugging purposes.
                    // TODO: We should check and make sure this is finding objects in all active scenes not just the game loader scene.
                    var playerInScene = FindFirstObjectByType<PlayerController>();

                    if (playerInScene != null)
                    {
                        _spawnedPlayerController = playerInScene;
                    }
                    // If there was none in the scene it is safe to spawn an instance of the _playerPrefab if it was set.
                    else if (playerInScene == null && _playerPrefab != null)
                    {
                        _spawnedPlayerController = Instantiate(_playerPrefab);
                    }
                } // End of outermost if statement.

                return _spawnedPlayerController;
            }
            private set
            {
                if (value != null)
                    _spawnedPlayerController = value;
            }
        }
        
        [SerializeField] private GameObject _hudPrefab;
        public GameObject SpawnedHUD { get; private set; }
        
        private static LevelPlayData _instance;
        public static LevelPlayData Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                if (_instance != null || value == null)
                    return;
                
                _instance = value;
            }
        }
        
        // Runs when the SO Asset is created.
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        
        /// <summary>
        ///  Runs when the SO Asset is loaded into memory the first time.
        /// </summary>
        private void OnEnable()
        {
            if (Instance == null)
                Instance = this;
        }

#if UNITY_EDITOR
        /* Only do OnValidate in editor.
            The idea is in editor when we set the LevelPlayData to the field we assign the
            static instance of it that is used during Runtime. */
        
        
        [SerializeField] private LevelPlayData _playDataObj;
        
        private void OnValidate()
        {
            _instance = _playDataObj;
        }
#endif
    }
}
