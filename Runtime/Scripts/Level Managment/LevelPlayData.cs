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
        [field:SerializeField] public GameObject PlayerPrefab { get; private set; }
        
        [SerializeField] private GameObject _hudPrefab;
        public GameObject SpawnedHUD { get; private set; }

        public int StartingRoomID;
        
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
    }
}
