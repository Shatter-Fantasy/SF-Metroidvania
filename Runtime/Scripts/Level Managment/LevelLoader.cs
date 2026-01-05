using System;
using SF.CameraModule;
using SF.RoomModule;
using SF.SpawnModule;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SF.LevelModule
{
    /// <summary>
    /// Loads the required game objects for used in managers and core systems in playable levels.
    /// </summary>
    [DefaultExecutionOrder(-4)]
    public class LevelLoader : MonoBehaviour
    {
        /// <summary>
        /// These are the indexes that are considered not part of a playable game scene.
        /// Any scene that has an index in this array will not start the First Playable Scene Loaded sequence.
        /// </summary>
        [Header("Level Initialization")] [SerializeField]
        private int[] _gameStartingSceneIndexes = new int[1];

        [SerializeField] private LevelPlayData _levelData;
        /// <summary>
        /// Is called when a level starts to load up. 
        /// </summary>
        public static event Action LevelLoadingHandler;
        
        /// <summary>
        /// This is called when the first playable is ready to give the player control.
        /// </summary>
        public static event Action LevelReadyHandler;

        private void Awake()
        {
            // This has to be done in awake. OnEnable/Start is called after the first sceneLoaded call.
            SceneManager.sceneLoaded += OnLevelLoaded;
        }

        private void Start()
        {
            RoomSystem.SetInitialRoom(_levelData.StartingRoomID);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnLevelLoaded;
        }
        
        /// <summary>
        /// Loads all the required game objects used by managers and the core systems in a level so they can be used.
        /// Called by the SceneManager when any scene is loaded.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnLevelLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            LevelLoadingHandler?.Invoke();
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
        
        /// <summary>
        /// Initialize the level related game data.
        /// </summary>
        /// <remarks>
        /// Do not spawn any game objects in scene here. Spawning game objects before the <see cref="SceneManager.sceneLoaded"/>
        /// is finished will cause newly spawned objects to not show in the hierarchy view.  
        /// </remarks>
        private void PlayableGameSceneInitialization()
        {
            if (_levelData != null)
            {
                LevelPlayData.Instance = _levelData;
            }

            LevelReadyHandler?.Invoke();
        }
    }
}
